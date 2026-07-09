namespace UmaFestHub.Web.ViewModels;

// Minimal POST payload for creating a review.
// We intentionally do NOT accept UserId/Film ids from the client to prevent tampering;
// the controller derives them from the authenticated user and the FestivalFilm record.
public sealed class CreateReviewInputModel
{
	public Guid FestivalFilmId { get; set; }
	public int Rating { get; set; }
	public string Comment { get; set; } = string.Empty;
}

