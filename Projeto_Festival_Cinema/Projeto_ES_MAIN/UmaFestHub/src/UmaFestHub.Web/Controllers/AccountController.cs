using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Application.Strategies;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.Services;

namespace UmaFestHub.Web.Controllers;

public class AccountController : Controller
{
	private readonly IUserService _userService;
	private readonly ICookieSignInService _cookieSignInService;
	private readonly IEnumerable<IUserRoleStrategy> _strategies;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public AccountController(
		IUserService userService,
		ICookieSignInService cookieSignInService,
		IEnumerable<IUserRoleStrategy> strategies,
		IStringLocalizer<SharedResources> localizer)
	{
		_userService = userService;
		_cookieSignInService = cookieSignInService;
		_strategies = strategies;
		_localizer = localizer;
	}

	[HttpGet("/login", Name = "Login")]
	public IActionResult Login() => View(new UserViewModel());

	[HttpGet("/register", Name = "Register")]
	public IActionResult Register() => View(new UserViewModel());

	[HttpGet("/forgot-password", Name = "ForgotPassword")]
	public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

	[HttpPost("/forgot-password", Name = "ForgotPasswordPost")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		try
		{
			var token = await _userService.GeneratePasswordResetTokenAsync(model.Email, cancellationToken);
			if (token is null)
			{
				ModelState.AddModelError(string.Empty, _localizer["Account_EmailNotFound"].Value);
				return View(model);
			}

			TempData["ResetToken"] = token;
			TempData["ResetEmail"] = model.Email;

			return RedirectToRoute("ResetPassword");
		}
		catch
		{
			ModelState.AddModelError(string.Empty, _localizer["Account_AuthUnavailable"].Value);
			return View(model);
		}
	}

	[HttpGet("/reset-password", Name = "ResetPassword")]
	public IActionResult ResetPassword()
	{
		ViewBag.ResetToken = TempData["ResetToken"]?.ToString();
		ViewBag.ResetEmail = TempData["ResetEmail"]?.ToString();
		return View(new ResetPasswordViewModel());
	}

	[HttpPost("/reset-password", Name = "ResetPasswordPost")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.ResetToken = model.Token;
			ViewBag.ResetEmail = model.Email;
			return View(model);
		}

		try
		{
			var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword, cancellationToken);
			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, _localizer.LocalizeKeyOrFallback(result.Error, "Account_ResetFailed"));
				ViewBag.ResetToken = model.Token;
				ViewBag.ResetEmail = model.Email;
				return View(model);
			}

			TempData["SuccessMessage"] = _localizer["Account_ResetSuccess"].Value;
			return RedirectToRoute("Login");
		}
		catch
		{
			ModelState.AddModelError(string.Empty, _localizer["Account_AuthUnavailable"].Value);
			ViewBag.ResetToken = model.Token;
			ViewBag.ResetEmail = model.Email;
			return View(model);
		}
	}

	[HttpPost("/register", Name = "RegisterPost")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(UserViewModel model, CancellationToken cancellationToken)
	{
		try
		{
			var register = await _userService.RegisterAsync(
				model.Email,
				model.Password,
				model.FullName,
				asAdmin: false,
				cancellationToken);

			if (!register.Succeeded)
			{
				ModelState.AddModelError(string.Empty, _localizer.LocalizeKeyOrFallback(register.Error, "Account_RegisterFailed"));
				return View(model);
			}

			var user = await _userService.AuthenticateAsync(model.Email, model.Password, cancellationToken);
			if (user is null)
			{
				ModelState.AddModelError(string.Empty, _localizer["Account_SignInAfterRegisterFailed"].Value);
				return View(model);
			}

			await _cookieSignInService.SignInAsync(user.Id, user.Email, user.Name, user.Roles, cancellationToken);
			return RedirectByRoles(user.Roles);
		}
		catch
		{
			ModelState.AddModelError(string.Empty, _localizer["Account_AuthUnavailable"].Value);
			return View(model);
		}
	}

	[HttpPost("/login", Name = "LoginPost")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(UserViewModel model, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _userService.AuthenticateAsync(model.Email, model.Password, cancellationToken);
			if (user is null)
			{
				ModelState.AddModelError(string.Empty, _localizer["Account_InvalidCredentials"].Value);
				return View(model);
			}

			await _cookieSignInService.SignInAsync(user.Id, user.Email, user.Name, user.Roles, cancellationToken);
			return RedirectByRoles(user.Roles);
		}
		catch
		{
			ModelState.AddModelError(string.Empty, _localizer["Account_AuthUnavailable"].Value);
			return View(model);
		}
	}

	[HttpGet("/logout", Name = "Logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToRoute("HomeIndex");
	}

	[HttpGet("/account/access-denied", Name = "AccessDenied")]
	public IActionResult AccessDenied()
	{
		return Redirect("/?toast=access-denied");
	}

	// private IActionResult RedirectByRoles(IReadOnlyList<string> roles)
	// {
	// 	if (roles.Any(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)))
	// 	{
	// 		return RedirectToRoute("AdminIndex");
	// 	}

	// 	if (roles.Any(role => string.Equals(role, "Organizer", StringComparison.OrdinalIgnoreCase)))
	// 	{
	// 		return RedirectToRoute("ManageIndex");
	// 	}

	// 	return RedirectToRoute("HomeIndex");
	// }

	private IActionResult RedirectByRoles(IReadOnlyList<string> roles)
	{
		var context = new UserContext(_strategies, roles.FirstOrDefault() ?? "");
		var destination = context.GetRedirectDestination();
		var routeName = RedirectDestinationMapper.ToRouteName(destination);
		return RedirectToRoute(routeName);
	}
}
