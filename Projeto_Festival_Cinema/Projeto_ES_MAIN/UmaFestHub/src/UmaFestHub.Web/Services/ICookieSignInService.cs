namespace UmaFestHub.Web.Services;

/// <summary>Issues or refreshes the auth cookie so <see cref="System.Security.Claims.ClaimsPrincipal"/> reflects current user data.</summary>
public interface ICookieSignInService
{
	Task SignInAsync(Guid userId, string email, string name, IReadOnlyList<string> roles, CancellationToken cancellationToken = default);
}
