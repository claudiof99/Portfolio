// -----------------------------------------------------------------------------
// Awards, nominations & votes — One “start award → nominees” GET form per category
// (shared markup via _AwardCategoryStartForm.cshtml).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Web.ViewModels;

public sealed class AwardCategoryStartFormViewModel
{
	public string Heading { get; init; } = string.Empty;
	public int Category { get; init; }
	public string CategoryLabel { get; init; } = string.Empty;
	public string AwardNamePlaceholder { get; init; } = string.Empty;
	public string FestivalRadioIdPrefix { get; init; } = string.Empty;
	public bool SectionTopMargin { get; init; }
	public IReadOnlyList<FestivalOptionViewModel> Festivals { get; init; } = Array.Empty<FestivalOptionViewModel>();

	public static IReadOnlyList<AwardCategoryStartFormViewModel> CreateDefaultSet(
		IReadOnlyList<FestivalOptionViewModel> festivals)
	{
		return
		[
			new AwardCategoryStartFormViewModel
			{
				Heading = "Film",
				Category = (int)AwardCategory.Film,
				CategoryLabel = nameof(AwardCategory.Film),
				AwardNamePlaceholder = "Award name (e.g., Best Picture)",
				FestivalRadioIdPrefix = "films-festival",
				SectionTopMargin = true,
				Festivals = festivals
			},
			new AwardCategoryStartFormViewModel
			{
				Heading = "Actor",
				Category = (int)AwardCategory.Actor,
				CategoryLabel = nameof(AwardCategory.Actor),
				AwardNamePlaceholder = "Award name (e.g., Outstanding Performance)",
				FestivalRadioIdPrefix = "actors-festival",
				Festivals = festivals
			},
			new AwardCategoryStartFormViewModel
			{
				Heading = "Director",
				Category = (int)AwardCategory.Director,
				CategoryLabel = nameof(AwardCategory.Director),
				AwardNamePlaceholder = "Award name (e.g., Best Director)",
				FestivalRadioIdPrefix = "director-festival",
				Festivals = festivals
			},
			new AwardCategoryStartFormViewModel
			{
				Heading = "Writing",
				Category = (int)AwardCategory.Writing,
				CategoryLabel = nameof(AwardCategory.Writing),
				AwardNamePlaceholder = "Award name (e.g., Best Screenplay)",
				FestivalRadioIdPrefix = "writing-festival",
				Festivals = festivals
			}
		];
	}
}
