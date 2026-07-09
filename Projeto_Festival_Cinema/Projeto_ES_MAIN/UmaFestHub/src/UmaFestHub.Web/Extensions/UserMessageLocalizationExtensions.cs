using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Extensions;

public static class UserMessageLocalizationExtensions
{
	public static string Localize(this IStringLocalizer<SharedResources> localizer, UserMessage message)
	{
		if (message.Key == UserMessageKeys.Cart_DuplicateItem && message.Args.Length > 0)
		{
			var productType = localizer.LocalizeProductType(message.Args[0]?.ToString() ?? string.Empty);
			return localizer[message.Key, productType].Value;
		}

		return message.Args.Length == 0
			? localizer[message.Key].Value
			: localizer[message.Key, message.Args].Value;
	}

	public static string LocalizeProductType(this IStringLocalizer<SharedResources> localizer, string? productType)
	{
		var key = productType switch
		{
			"DailyPass" => "ProductType_DailyPass",
			"CompletePass" => "ProductType_CompletePass",
			"Rental" => "ProductType_Rental",
			"Ticket" => "ProductType_Ticket",
			_ => null,
		};

		return key is null ? (productType ?? string.Empty) : localizer[key].Value;
	}

	public static string LocalizeSessionType(this IStringLocalizer<SharedResources> localizer, string? sessionType)
	{
		if (string.IsNullOrWhiteSpace(sessionType))
		{
			return string.Empty;
		}

		var normalized = sessionType.Trim();
		var key = normalized switch
		{
			"Fixed" or "FixedSession" => "Session_TypeFixed",
			"Premier" or "PremierSession" => "Session_TypePremier",
			"AccessWindow" or "AccessWindowSession" => "Session_TypeAccessWindow",
			"Pass Access" => "Session_TypePassAccess",
			_ => null,
		};

		return key is null ? normalized : localizer[key].Value;
	}

	public static string LocalizeAwardCategory(this IStringLocalizer<SharedResources> localizer, string? category)
	{
		var key = category switch
		{
			"Film" => "Award_CategoryFilm",
			"Actor" => "Award_CategoryActor",
			"Director" => "Award_CategoryDirector",
			"Writing" => "Award_CategoryWriting",
			_ => null,
		};

		return key is null ? (category ?? string.Empty) : localizer[key].Value;
	}

	public static string LocalizeRentalUnit(this IStringLocalizer<SharedResources> localizer, string? unit)
	{
		var key = unit switch
		{
			"Minutes" => "Rental_UnitMinutes",
			"Hours" => "Rental_UnitHours",
			"Days" => "Rental_UnitDays",
			_ => null,
		};

		return key is null ? (unit ?? string.Empty) : localizer[key].Value;
	}

	public static string LocalizeDisplayText(this IStringLocalizer<SharedResources> localizer, string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}

		var localized = localizer[text];
		return localized.ResourceNotFound ? text : localized.Value;
	}

	public static string LocalizeKeyOrFallback(
		this IStringLocalizer<SharedResources> localizer,
		string? key,
		string fallbackKey = "Common_UnexpectedError")
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return localizer[fallbackKey].Value;
		}

		var localized = localizer[key];
		return localized.ResourceNotFound ? localizer[fallbackKey].Value : localized.Value;
	}

	public static string LocalizeUserFacing(this IStringLocalizer<SharedResources> localizer, Exception exception)
	{
		if (exception is UserFacingException userFacing)
		{
			return string.Join("; ", userFacing.Messages.Select(m => localizer.Localize(m)));
		}

		return localizer.LocalizeKeyOrFallback(exception.Message);
	}

	public static string LocalizeUserFacing(this IStringLocalizer<SharedResources> localizer, UserMessage? message)
		=> message is null ? string.Empty : localizer.Localize(message);
}

