using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Interfaces;

/// <summary>
/// Application entry for user film lists (watchlist, favorites, watched). Delegates to the personal list repository implementation.
/// </summary>
public interface IPersonalListService
{
	Task AddFilmAsync(Guid userId, PersonalListType type, Guid filmId, CancellationToken cancellationToken = default);

	Task RemoveFilmAsync(Guid userId, PersonalListType type, Guid filmId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Guid>> GetListAsync(Guid userId, PersonalListType type, CancellationToken cancellationToken = default);
}
