// -----------------------------------------------------------------------------
// Review replies — form model POSTed to Review/Reply (anti-forgery + server-side film check).
// -----------------------------------------------------------------------------
namespace UmaFestHub.Web.ViewModels;

/// <summary>POST body for adding a reply; <see cref="FestivalFilmId"/> is checked against the parent review server-side.</summary>
public sealed class AddReviewReplyInputModel
{
	public Guid ReviewId { get; set; }
	public Guid FestivalFilmId { get; set; }
	public string Comment { get; set; } = string.Empty;
}
