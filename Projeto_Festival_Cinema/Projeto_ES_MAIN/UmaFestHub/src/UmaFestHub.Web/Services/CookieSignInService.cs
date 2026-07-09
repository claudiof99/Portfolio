using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Web.Services;

public sealed class CookieSignInService(IHttpContextAccessor httpContextAccessor) : ICookieSignInService
{
	public async Task SignInAsync(
		Guid userId,
		string email,
		string name,
		IReadOnlyList<string> roles,
		CancellationToken cancellationToken = default)
	{
		var httpContext = httpContextAccessor.HttpContext
			?? throw new InvalidOperationException("No active HTTP context.");

		var normalizedRoles = roles
			.Where(r => !string.IsNullOrWhiteSpace(r))
			.Select(r => r.Trim())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (normalizedRoles.Count == 0)
		{
			normalizedRoles.Add(UserRole.Customer.ToString());
		}

		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, userId.ToString()),
			new(ClaimTypes.Email, email),
			new(ClaimTypes.Name, string.IsNullOrWhiteSpace(name) ? email : name),
			new("sub", userId.ToString()),
		};

		claims.AddRange(normalizedRoles.Select(role => new Claim(ClaimTypes.Role, role)));

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
	}
}
