using System.Globalization;

namespace UmaFestHub.Web.Extensions;

public static class LocalizationExtensions
{
	/// <summary>
	/// Neutral cultures (en, pt, fr) use the generic currency sign (¤). Map to regional cultures for dates,
	/// with USD ($) currency formatting across all locales.
	/// </summary>
	public static CultureInfo MapToRegionalCulture(CultureInfo culture)
	{
		var baseCulture = culture.TwoLetterISOLanguageName switch
		{
			"pt" => CultureInfo.GetCultureInfo("pt-PT"),
			"fr" => CultureInfo.GetCultureInfo("fr-FR"),
			_ => CultureInfo.GetCultureInfo("en-US"),
		};

		return ApplyUsdCurrencyFormat(baseCulture);
	}

	private static CultureInfo ApplyUsdCurrencyFormat(CultureInfo culture)
	{
		var usd = CultureInfo.GetCultureInfo("en-US");
		var result = (CultureInfo)culture.Clone();
		result.NumberFormat.CurrencySymbol = usd.NumberFormat.CurrencySymbol;
		result.NumberFormat.CurrencyDecimalDigits = usd.NumberFormat.CurrencyDecimalDigits;
		result.NumberFormat.CurrencyDecimalSeparator = usd.NumberFormat.CurrencyDecimalSeparator;
		result.NumberFormat.CurrencyGroupSeparator = usd.NumberFormat.CurrencyGroupSeparator;
		result.NumberFormat.CurrencyPositivePattern = usd.NumberFormat.CurrencyPositivePattern;
		result.NumberFormat.CurrencyNegativePattern = usd.NumberFormat.CurrencyNegativePattern;
		return result;
	}

	public static string FormatCurrency(this decimal amount)
		=> amount.ToString("C", MapToRegionalCulture(CultureInfo.CurrentUICulture));

	public static string CurrentCultureCode(this HttpContext context)
	{
		var feature = context.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
		return feature?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
	}

	public static string CurrentCultureLabel(this HttpContext context)
		=> context.CurrentCultureCode().ToUpperInvariant();
}
