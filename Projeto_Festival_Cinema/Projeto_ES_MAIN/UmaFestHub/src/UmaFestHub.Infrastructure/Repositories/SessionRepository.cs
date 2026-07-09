using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
	private readonly AppDbContext _dbContext;

	public SessionRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _dbContext.Sessions
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task<IReadOnlyList<Session>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Sessions
			.AsNoTracking()
			.OrderBy(x => x.StartTimeUtc)
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Session>> GetByFestivalFilmIdAsync(Guid festivalFilmId, CancellationToken cancellationToken = default)
		=> await _dbContext.Sessions
			.Where(x => x.FestivalFilmId == festivalFilmId)
			.AsNoTracking()
			.OrderBy(x => x.StartTimeUtc)
			.ToListAsync(cancellationToken);

	public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
	{
		await _dbContext.Sessions.AddAsync(session, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}