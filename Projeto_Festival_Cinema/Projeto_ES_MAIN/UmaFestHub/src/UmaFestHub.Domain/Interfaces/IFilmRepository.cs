using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface IFilmRepository
{
	Task<IReadOnlyList<Film>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Film>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
	Task AddAsync(Film film, CancellationToken cancellationToken = default);
	Task<Film?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
