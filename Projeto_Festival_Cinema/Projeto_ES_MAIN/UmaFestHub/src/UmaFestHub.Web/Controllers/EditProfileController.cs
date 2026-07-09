using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Validation;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Services;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

[Authorize]
public class EditProfileController : Controller
{
	private readonly IUserService _userService;
	private readonly ICookieSignInService _cookieSignInService;
	private readonly IViewModelValidator<EditProfileViewModel> _viewModelValidator;

	public EditProfileController(
		IUserService userService,
		ICookieSignInService cookieSignInService,
		IViewModelValidator<EditProfileViewModel> viewModelValidator)
	{
		_userService = userService;
		_cookieSignInService = cookieSignInService;
		_viewModelValidator = viewModelValidator;
	}

	[HttpGet("/edit-profile")]
	public IActionResult Index()
	{
		return View();
	}

	[HttpPost("/edit-profile")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Post(EditProfileViewModel model, CancellationToken cancellationToken = default)
	{
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		_viewModelValidator.Validate(model, new ModelStateWrapper(ModelState));
		if (!ModelState.IsValid)
		{
			return View("Index", model);
		}

		var updated = await _userService.UpdateUserProfileAsync(userId, model.NewName, model.NewEmail, cancellationToken);
		if (updated is null)
		{
			return NotFound();
		}

		await _cookieSignInService.SignInAsync(updated.Id, updated.Email, updated.Name, updated.Roles, cancellationToken);
		return RedirectToAction("Index", "Profile");
	}
}
