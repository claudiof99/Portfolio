using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class PersonalListRepository : IPersonalListRepository
{
	private readonly AppDbContext _dbContext;

	public PersonalListRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task AddAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default)
	{
		var exists = await _dbContext.PersonalLists.AnyAsync(
			x => x.UserId == userId && x.Type == type && x.FilmId == filmId,
			cancellationToken);

		if (exists)
		{
			return;
		}

		_dbContext.PersonalLists.Add(new PersonalList
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Type = type,
			FilmId = filmId
		});

		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task RemoveAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default)
	{
		var entity = await _dbContext.PersonalLists.FirstOrDefaultAsync(
			x => x.UserId == userId && x.Type == type && x.FilmId == filmId,
			cancellationToken);

		if (entity is null)
		{
			return;
		}

		_dbContext.PersonalLists.Remove(entity);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Guid>> GetByUserAndTypeAsync(Guid userId, PersonalListType type, CancellationToken cancellationToken = default)
		=> await _dbContext.PersonalLists
			.AsNoTracking()
			.Where(x => x.UserId == userId && x.Type == type)
			.Select(x => x.FilmId)
			.ToListAsync(cancellationToken);

	public async Task<bool> ExistsAsync(Guid userId, Guid filmId, PersonalListType type, CancellationToken cancellationToken = default)
		=> await _dbContext.PersonalLists.AnyAsync(
			x => x.UserId == userId && x.Type == type && x.FilmId == filmId,
			cancellationToken);

	public async Task<IReadOnlyList<PersonalList>> GetFullByUserAndTypeAsync(
		Guid userId,
		PersonalListType type,
		CancellationToken cancellationToken = default)
		=> await _dbContext.PersonalLists
			.AsNoTracking()
			.Include(x => x.Film)
				.ThenInclude(f => f!.Genres)
			.Where(x => x.UserId == userId && x.Type == type)
			.ToListAsync(cancellationToken);
}
