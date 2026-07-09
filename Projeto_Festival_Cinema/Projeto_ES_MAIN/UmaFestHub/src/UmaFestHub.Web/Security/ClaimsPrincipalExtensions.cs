using System.Security.Claims;

namespace UmaFestHub.Web.Security;

public static class ClaimsPrincipalExtensions
{
	// Helper for runtime role checks without hardcoding role names in controllers/views.
	public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
	{
		if (principal is null || roles is null || roles.Length == 0)
		{
			return false;
		}

		foreach (var role in roles)
		{
			if (!string.IsNullOrWhiteSpace(role) && principal.IsInRole(role))
			{
				return true;
			}
		}

		return false;
	}
}

