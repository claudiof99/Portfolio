using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;
using UmaFestHub.Domain.ValueObjects;

namespace UmaFestHub.Infrastructure.Repositories;

public sealed class CreditRepository : ICreditRepository
{
	private readonly AppDbContext _dbContext;

	public CreditRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<CreditFilm>> GetByFestivalAndRoleAsync(Guid festivalId, string role, CancellationToken cancellationToken = default)
	{
		var normalizedRole = (role ?? string.Empty).Trim();

		// Restrict credits to films that are in the selected festival lineup.
		return await _dbContext.Credits
			.Include(c => c.Person)
			.Where(c => c.Role == normalizedRole)
			.Join(
				_dbContext.FestivalFilms.Where(ff => ff.FestivalId == festivalId),
				credit => credit.FilmId,
				ff => ff.FilmId,
				(credit, _) => credit)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}
}

