namespace UmaFestHub.Web.ViewModels;

// ViewModel for the staff moderation page (Review/Manage).
// Stores paging state plus the active filters so the UI and pager can preserve them.
public sealed class ReviewManagePageViewModel
{
	public int Page { get; set; } = 1;
	public bool HasNext { get; set; }
	public IReadOnlyList<ReviewViewModel> Reviews { get; set; } = Array.Empty<ReviewViewModel>();

	public string? MovieQuery { get; set; }
	public string? AuthorQuery { get; set; }
	public string? Status { get; set; }
	public DateTime? DayUtc { get; set; }
}

