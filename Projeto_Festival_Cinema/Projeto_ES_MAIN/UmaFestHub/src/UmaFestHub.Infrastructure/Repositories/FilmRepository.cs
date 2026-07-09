using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

/// <summary>
/// We use this repository to handle all data access operations related to Films, ensuring optimal query performance.
/// </summary>
public class FilmRepository : IFilmRepository
{
	private readonly AppDbContext _dbContext;

	/// <summary>
	/// We lookup a film by its external TMDb ID. This is critical for our deduplication logic to prevent importing the same movie twice.
	/// </summary>
	public async Task<Film?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Films
			.Include(x => x.Genres)
			.Include(x => x.Credits)
				.ThenInclude(c => c.Person)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);
	}
	public FilmRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <summary>
	/// We retrieve all films from the database, pulling in associated genres as read-only tracking for speed.
	/// </summary>
	public async Task<IReadOnlyList<Film>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Films
			.Include(x => x.Genres)
			.Include(x => x.Credits)
				.ThenInclude(c => c.Person)
			.AsNoTracking()
			.OrderBy(x => x.Name)
			.ToListAsync(cancellationToken);

	/// <summary>
	/// We fetch a specific film by its internal Guid, including all its genres and cast credits.
	/// </summary>
	public async Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _dbContext.Films
			.Include(x => x.Genres)
			.Include(x => x.Credits)
				.ThenInclude(c => c.Person)
			.Include(x => x.FestivalFilms)
				.ThenInclude(ff => ff.Festival)
			.Include(x => x.FestivalFilms)
				.ThenInclude(ff => ff.Sessions)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task<IReadOnlyList<Film>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
	{
		if (ids.Count == 0)
		{
			return Array.Empty<Film>();
		}

		return await _dbContext.Films
			.Include(x => x.Genres)
			.Where(x => ids.Contains(x.Id))
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	/// <summary>
	/// We insert a newly mapped film entity into the database.
	/// </summary>
	public async Task AddAsync(Film film, CancellationToken cancellationToken = default)
	{
		await _dbContext.Films.AddAsync(film, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// We delete a film from the database.
	/// </summary>
	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var film = await _dbContext.Films.FindAsync(new object[] { id }, cancellationToken);
		if (film != null)
		{
			_dbContext.Films.Remove(film);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
