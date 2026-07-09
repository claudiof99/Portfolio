// -----------------------------------------------------------------------------
// Awards, nominations & votes — EF implementation of INominationRepository.
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class NominationRepository : INominationRepository
{
	private readonly AppDbContext _dbContext;

	public NominationRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<AwardNomination>> GetByAwardIdAsync(Guid awardId, CancellationToken cancellationToken = default)
		=> await _dbContext.AwardNominations
			.Where(x => x.AwardId == awardId)
			.Include(x => x.Votes)
			.Include(x => x.FestivalFilm).ThenInclude(ff => ff!.Film)
			.Include(x => x.CreditFilm).ThenInclude(c => c!.Person)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

	public async Task<AwardNomination?> GetByIdWithVotesAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _dbContext.AwardNominations
			.Include(x => x.Votes)
			.Include(x => x.FestivalFilm).ThenInclude(ff => ff!.Film)
			.Include(x => x.CreditFilm).ThenInclude(c => c!.Person)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task AddAsync(AwardNomination nomination, CancellationToken cancellationToken = default)
	{
		await _dbContext.AwardNominations.AddAsync(nomination, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(AwardNomination nomination, CancellationToken cancellationToken = default)
	{
		_dbContext.AwardNominations.Update(nomination);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
