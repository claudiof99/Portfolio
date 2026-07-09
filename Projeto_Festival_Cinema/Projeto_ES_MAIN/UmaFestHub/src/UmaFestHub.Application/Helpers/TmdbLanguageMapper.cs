using System.Globalization;

namespace UmaFestHub.Application.Helpers;

public static class TmdbLanguageMapper
{
	public static string ToTmdbLanguage(CultureInfo culture)
		=> ToTmdbLanguage(culture.TwoLetterISOLanguageName);

	public static string ToTmdbLanguage(string? cultureName)
	{
		if (string.IsNullOrWhiteSpace(cultureName))
		{
			return "en-US";
		}

		var twoLetter = cultureName.Length >= 2
			? cultureName[..2].ToLowerInvariant()
			: cultureName.ToLowerInvariant();

		return twoLetter switch
		{
			"pt" => "pt-PT",
			"fr" => "fr-FR",
			_ => "en-US",
		};
	}
}
