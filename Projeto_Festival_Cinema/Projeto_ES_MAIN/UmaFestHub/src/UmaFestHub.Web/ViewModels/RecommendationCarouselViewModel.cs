namespace UmaFestHub.Web.ViewModels;

public sealed class RecommendationCarouselViewModel
{
	public required string SectionId { get; init; }
	public required string SectionTitle { get; init; }
	public string? SectionTitleHref { get; init; }
	public required string RowElementId { get; init; }
	public required IReadOnlyList<RecommendationViewModel> Recommendations { get; init; }
	public required string EmptyMessage { get; init; }
	public required string ScrollLeftAriaLabel { get; init; }
	public required string ScrollRightAriaLabel { get; init; }
}