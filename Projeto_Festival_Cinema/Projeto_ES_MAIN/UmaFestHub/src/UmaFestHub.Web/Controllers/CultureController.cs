using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace UmaFestHub.Web.Controllers;

public sealed class CultureController : Controller
{
	private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase) { "en", "pt", "fr" };

	[HttpPost("/set-culture")]
	[ValidateAntiForgeryToken]
	public IActionResult SetCulture(string culture, string? returnUrl)
	{
		if (!Supported.Contains(culture))
		{
			culture = "en";
		}

		Response.Cookies.Append(
			CookieRequestCultureProvider.DefaultCookieName,
			CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
			new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddYears(1),
				IsEssential = true,
				HttpOnly = false
			});

		if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
		{
			returnUrl = "/";
		}

		return LocalRedirect(returnUrl);
	}
}
