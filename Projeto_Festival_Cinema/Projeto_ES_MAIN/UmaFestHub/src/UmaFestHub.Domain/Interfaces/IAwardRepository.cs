// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain persistence port
// Awards with nominations for listing, paging, festival filter, and IsActive updates.
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface IAwardRepository
{
	Task<(IReadOnlyList<Award> Items, bool HasNext)> GetPageWithNominationsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Award>> GetAllWithNominationsAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Award>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default);
	Task<Award?> GetByIdAsync(Guid awardId, CancellationToken cancellationToken = default);
	Task<Award?> GetByIdWithNominationsAsync(Guid awardId, CancellationToken cancellationToken = default);
	Task AddAsync(Award award, CancellationToken cancellationToken = default);
	Task<bool> TrySetIsActiveAsync(Guid awardId, bool isActive, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Guid>> GetActiveAwardIdsPastEndDateAsync(DateTime utcNow, CancellationToken cancellationToken = default);
	Task<IReadOnlyDictionary<Guid, int>> GetNominationCountsByFilmIdsAsync(Guid festivalId, IReadOnlyList<Guid> filmIds, CancellationToken cancellationToken = default);
}
