// -----------------------------------------------------------------------------
// Awards, nominations & votes — Row + nested nominee rows for award list/detail views.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Web.ViewModels;

public sealed class AwardNomineeRowViewModel
{
	public string Label { get; set; } = string.Empty;
	public int VoteCount { get; set; }
	public int VotePercentage { get; set; }
	public string? ImageUrl { get; set; }
}

public sealed class AwardViewModel
{
	public Guid Id { get; set; }
	public Guid FestivalId { get; set; }
	public string FestivalName { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public int NominationCount { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime EndDateUtc { get; set; }
	public int DaysRemaining { get; set; }
	public IReadOnlyList<AwardNomineeRowViewModel> Nominees { get; set; } = Array.Empty<AwardNomineeRowViewModel>();
}
