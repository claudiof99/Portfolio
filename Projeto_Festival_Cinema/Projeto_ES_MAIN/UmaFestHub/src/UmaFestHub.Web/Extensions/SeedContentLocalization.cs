using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Constants;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Extensions;

public static class SeedContentLocalization
{
	public static readonly Guid UmaSpringFestId = SeedContentIds.UmaSpringFestId;
	public static readonly Guid MidnightFramesFilmId = SeedContentIds.MidnightFramesFilmId;
	public static string LocalizeFestivalDescription(
		this IStringLocalizer<SharedResources> localizer,
		Guid festivalId,
		string? description)
	{
		if (festivalId == UmaSpringFestId)
		{
			return localizer["SeedFestival_UmaSpringFest_Description"].Value;
		}

		return description ?? string.Empty;
	}

	public static string LocalizeFilmDescription(
		this IStringLocalizer<SharedResources> localizer,
		Guid filmId,
		string? description)
	{
		if (filmId == MidnightFramesFilmId)
		{
			return localizer["SeedFilm_MidnightFrames_Description"].Value;
		}

		return description ?? string.Empty;
	}

	public static string LocalizeCreditRole(this IStringLocalizer<SharedResources> localizer, string? role)
	{
		if (string.IsNullOrWhiteSpace(role))
		{
			return localizer["Common_Unknown"].Value;
		}

		return role.Trim() switch
		{
			"Director" => localizer["CreditRole_Director"].Value,
			"Actor" => localizer["CreditRole_Actor"].Value,
			"Writer" => localizer["CreditRole_Writer"].Value,
			_ => role,
		};
	}
}
