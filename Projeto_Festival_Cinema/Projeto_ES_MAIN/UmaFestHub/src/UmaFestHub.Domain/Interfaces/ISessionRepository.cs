using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface ISessionRepository
{
	Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Session>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Session>> GetByFestivalFilmIdAsync(Guid festivalFilmId, CancellationToken cancellationToken = default);
	Task AddAsync(Session session, CancellationToken cancellationToken = default);
	
}
