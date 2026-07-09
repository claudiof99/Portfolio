// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain persistence port
// Award nominations and graph for voting (load with votes).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface INominationRepository
{
	Task<IReadOnlyList<AwardNomination>> GetByAwardIdAsync(Guid awardId, CancellationToken cancellationToken = default);
	Task<AwardNomination?> GetByIdWithVotesAsync(Guid id, CancellationToken cancellationToken = default);
	Task AddAsync(AwardNomination nomination, CancellationToken cancellationToken = default);
	Task UpdateAsync(AwardNomination nomination, CancellationToken cancellationToken = default);
}
