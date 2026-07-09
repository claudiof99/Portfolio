using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Web.ViewModels;

/// <summary>
/// Drives <c>_PersonalListCarousel.cshtml</c> on Home: horizontal film strip for watchlist, favorites, or Seen (<see cref="PersonalListType.Watched"/>).
/// Optional link to full list; remove posts to <c>PersonalListController.Remove</c>.
/// </summary>
public sealed class PersonalListCarouselViewModel
{
	public required string SectionId { get; init; }
	public required string SectionTitle { get; init; }
	/// <summary>
	/// If set, the section title links to the full list page (watchlist / favorites / Seen). For Seen, typically
	/// <c>/personal-list?type=Watched</c> — same backing rows as <c>/watchHistory</c>.
	/// </summary>
	public string? SectionTitleHref { get; init; }
	public required string RowElementId { get; init; }
	/// <summary>Which personal list row removals target (e.g. <see cref="PersonalListType.Watched"/> for Seen).</summary>
	public PersonalListType ListType { get; init; }
	public IReadOnlyList<FilmViewModel> Films { get; init; } = [];

	public required string EmptyMessage { get; init; }
	public required string RemoveTooltip { get; init; }
	public required string RemoveAriaLabel { get; init; }
	public required string ScrollLeftAriaLabel { get; init; }
	public required string ScrollRightAriaLabel { get; init; }
	public required string RemoveButtonGlyph { get; init; }

	/// <summary>If set, POST /PersonalList/Remove redirects here instead of home + #SectionId.</summary>
	public string? RemoveReturnUrl { get; init; }
}
