// -----------------------------------------------------------------------------
// Review replies — one thread row on Review/Index or under a card on Review/Manage.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Web.ViewModels;

/// <summary>One row in the reply thread under a review card.</summary>
public sealed class ReviewReplyViewModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string AuthorName { get; set; } = string.Empty;
	public string Comment { get; set; } = string.Empty;
	public DateTime DateUtc { get; set; }
	public string Status { get; set; } = string.Empty;
	public bool IsReported { get; set; }
	public bool HasBeenReported { get; set; }

	/// <summary>Organizer/Admin-authored reply; customers cannot report (same rule as reviews).</summary>
	public bool IsStaffAuthor { get; set; }

	/// <summary>Staff removed this reply from public listings (management UI).</summary>
	public bool IsHiddenByAdmin { get; set; }
}
