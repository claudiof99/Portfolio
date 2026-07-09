// -----------------------------------------------------------------------------
// Awards, nominations & votes — Award the user already voted on + their pick.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Application.DTOs;

public sealed class UserAwardVoteDto
{
	public AwardDto Award { get; init; } = null!;
	public Guid SelectedNominationId { get; init; }
}
