using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

/// <summary>
/// Persistence port for <see cref="PersonalList"/> rows: add/remove film membership and query film ids by user and list type.
/// </summary>
public interface IPersonalListRepository
{
	Task AddAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default);

	Task RemoveAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Guid>> GetByUserAndTypeAsync(Guid userId, PersonalListType type, CancellationToken cancellationToken = default);

	/// <summary>Returns PersonalList rows with Film and Genres eagerly loaded — for genre-based recommendations.</summary>
	Task<IReadOnlyList<PersonalList>> GetFullByUserAndTypeAsync(Guid userId, PersonalListType type, CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default);
}
