// -----------------------------------------------------------------------------
// Awards, nominations & votes — Application API for web layer (organizer + customer).
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces;

public interface IAwardService
{
	Task<(IReadOnlyList<AwardDto> Items, bool HasNext)> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<AwardDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<AwardDto>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<AwardDto>> GetByFestivalIdAvailableForVotingAsync(Guid festivalId, Guid userId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserAwardVoteDto>> GetVotedAwardsForFestivalAsync(Guid festivalId, Guid userId, CancellationToken cancellationToken = default);
	Task<Guid> CreateAsync(AwardDto award, CancellationToken cancellationToken = default);
	Task<Guid> NominateAsync(Guid awardId, Guid festivalFilmId, CancellationToken cancellationToken = default);
	Task<Guid> CreateWithNomineesAsync(
		Guid festivalId,
		string awardName,
		AwardCategory category,
		IReadOnlyList<Guid> nomineeIds,
		DateTime endDateUtc,
		CancellationToken cancellationToken = default);

	Task DeactivateAsync(Guid awardId, Guid deactivatedByUserId, CancellationToken cancellationToken = default);

	Task ExpireDueAwardsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Casts a vote for a nomination.
	/// </summary>
	Task VoteAsync(Guid userId, Guid nominationId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the winner for an award (nomination with highest votes).
	/// </summary>
	Task<AwardNomination?> GetWinnerAsync(Guid awardId, CancellationToken cancellationToken = default);
}
