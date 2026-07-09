// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain persistence port
// Vote persistence; helpers enforce one vote per user per award / nomination.
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces
{
    public interface IVoteRepository
    {
        Task<bool> HasVotedAsync(Guid userId, Guid nominationId, CancellationToken cancellationToken = default);
        Task<bool> HasVotedForAwardAsync(Guid userId, Guid awardId, CancellationToken cancellationToken = default);
        Task<IReadOnlySet<Guid>> GetVotedAwardIdsForFestivalAsync(Guid userId, Guid festivalId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, Guid>> GetUserVotedNominationIdsByAwardForFestivalAsync(Guid userId, Guid festivalId, CancellationToken cancellationToken = default);
        Task AddAsync(Vote vote, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Vote>> GetByNominationIdAsync(Guid nominationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, int>> GetVoteCountsByFilmIdsAsync(Guid festivalId, IReadOnlyList<Guid> filmIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Guid>> GetDistinctVoterUserIdsForAwardAsync(Guid awardId, CancellationToken cancellationToken = default);
    }
}