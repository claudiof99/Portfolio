namespace UmaFestHub.Web.ViewModels;

// ViewModel for the public reviews page of a single FestivalFilm.
// Contains paging state and the list of reviews already shaped for the UI.
public sealed class ReviewIndexPageViewModel
{
	public Guid FestivalFilmId { get; set; }
	public string FilmTitle { get; set; } = string.Empty;
	public string? FilmImageUrl { get; set; }
	public int Page { get; set; } = 1;
	public bool HasNext { get; set; }
	public IReadOnlyList<ReviewViewModel> Reviews { get; set; } = Array.Empty<ReviewViewModel>();
}
