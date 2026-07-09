namespace UmaFestHub.Web.ViewModels;

// UI shape for displaying a review card in both public (Index) and staff (Manage) pages.
// Replies: nested ReviewReplyViewModel list (thread + moderation actions in views).
// This is intentionally presentation-oriented (string Status, FilmTitle, computed flags, etc.).
public sealed class ReviewViewModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string AuthorName { get; set; } = string.Empty;
	public Guid? FestivalFilmId { get; set; }
	public Guid? FilmId { get; set; }
	public string FilmTitle { get; set; } = string.Empty;
	public int ExternalFilmId { get; set; }
	public int Rating { get; set; }
	public string Comment { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public DateTime DateUtc { get; set; }
	public bool IsReported { get; set; }
	public bool HasBeenReported { get; set; }

	// Used by the UI to disable reporting on staff-authored reviews (Organizer/Admin).
	public bool IsStaffAuthor { get; set; }

	/// <summary>Thread under this card (public Index; staff Manage includes all replies).</summary>
	public List<ReviewReplyViewModel> Replies { get; set; } = [];
}
