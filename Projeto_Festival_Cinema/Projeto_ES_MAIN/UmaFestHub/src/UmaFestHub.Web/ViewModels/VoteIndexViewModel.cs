// -----------------------------------------------------------------------------
// Awards, nominations & votes — Customer vote page: pending awards + completed votes.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Web.ViewModels;

public sealed class VoteIndexViewModel
{
	public Guid FestivalId { get; set; }
	public string FestivalName { get; set; } = string.Empty;
	public IReadOnlyList<AwardDto> Awards { get; set; } = Array.Empty<AwardDto>();
	public IReadOnlyList<UserAwardVoteDto> CompletedVotes { get; set; } = Array.Empty<UserAwardVoteDto>();
	public string? ErrorMessage { get; set; }
	public string? SuccessMessage { get; set; }
}
