using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Web.ViewModels;

/// <summary>Optional festival filter on <c>/personal-list</c>; id + display name for dropdown.</summary>
public sealed record PersonalListFestivalFilterOption(Guid Id, string Name);

/// <summary>
/// View model for GET <c>/personal-list</c> and GET <c>/watchHistory</c>: active list type, hero copy, filters, and resolved <see cref="FilmViewModel"/> cards.
/// When <see cref="ListType"/> is <see cref="PersonalListType.Watched"/>, the UI label is “Seen”.
/// </summary>
public sealed class PersonalListPageViewModel
{
	/// <summary>Active tab: watchlist, favorites, or Seen (<see cref="PersonalListType.Watched"/>).</summary>
	public PersonalListType ListType { get; init; }
	public required string PageTitle { get; init; }
	public required string PageSubtitle { get; init; }
	public IReadOnlyList<FilmViewModel> Films { get; init; } = [];

	/// <summary>Film count in this list before title/genre/festival filters.</summary>
	public int SourceListFilmCount { get; init; }

	public string? FilterTitle { get; init; }
	public string? FilterGenre { get; init; }
	public Guid? SelectedFestivalId { get; init; }

	/// <summary>Festivals whose lineup overlaps this list (narrowed by purchase history when the user has any).</summary>
	public IReadOnlyList<PersonalListFestivalFilterOption> FestivalFilterOptions { get; init; } = [];
}
