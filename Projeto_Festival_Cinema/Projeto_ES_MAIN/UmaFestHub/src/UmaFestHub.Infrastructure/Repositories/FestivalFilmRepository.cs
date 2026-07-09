using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using UmaFestHub.Domain.Entities;
    using UmaFestHub.Infrastructure.Data;

    /// <summary>
    /// We use this repository to manage the data access for FestivalFilms, bridging the many-to-many relationship in our database.
    /// </summary>
    public class FestivalFilmRepository : IFestivalFilmRepository
    {
        private readonly AppDbContext _dbContext;

        public FestivalFilmRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// We retrieve a specific FestivalFilm by its ID, eagerly loading its associated Film metadata.
        /// </summary>
        public async Task<FestivalFilm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbContext.FestivalFilms
                .Include(ff => ff.Film)
                .Include(ff => ff.Festival)
                .AsNoTracking()
                .FirstOrDefaultAsync(ff => ff.Id == id, cancellationToken);

        /// <summary>
        /// We fetch the entire lineup for a festival in one query, eagerly loading Films, Genres, and Sessions to prevent N+1 performance issues.
        /// </summary>
        public async Task<IReadOnlyList<FestivalFilm>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default)
            => await _dbContext.FestivalFilms
                .Include(ff => ff.Film)
                    .ThenInclude(f => f!.Genres)
                .Include(ff => ff.Sessions)
                .Where(ff => ff.FestivalId == festivalId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<(Guid FestivalId, string FestivalName)>> GetDistinctFestivalsContainingFilmIdsAsync(
            IReadOnlyCollection<Guid> filmIds,
            CancellationToken cancellationToken = default)
        {
            if (filmIds == null || filmIds.Count == 0)
            {
                return [];
            }

            var rows = await _dbContext.FestivalFilms
                .AsNoTracking()
                .Where(ff => filmIds.Contains(ff.FilmId))
                .Select(ff => new { ff.FestivalId, Name = ff.Festival!.Name })
                .Distinct()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return rows.Select(r => (r.FestivalId, r.Name)).ToList();
        }

        public async Task<IReadOnlySet<Guid>> GetFilmIdsInFestivalProgramAsync(Guid festivalId, CancellationToken cancellationToken = default)
        {
            var list = await _dbContext.FestivalFilms.AsNoTracking()
                .Where(ff => ff.FestivalId == festivalId)
                .Select(ff => ff.FilmId)
                .Distinct()
                .ToListAsync(cancellationToken);
            return list.ToHashSet();
        }

        public async Task<IReadOnlyDictionary<Guid, string?>> GetFirstImportedFilmImageUrlsByFestivalIdsAsync(
            IReadOnlyCollection<Guid> festivalIds,
            CancellationToken cancellationToken = default)
        {
            if (festivalIds == null || festivalIds.Count == 0)
            {
                return new Dictionary<Guid, string?>();
            }

            var lineup = await _dbContext.FestivalFilms
                .AsNoTracking()
                .Where(ff => festivalIds.Contains(ff.FestivalId))
                .Include(ff => ff.Film)
                .OrderBy(ff => ff.AddedAtUtc)
                .Select(ff => new { ff.FestivalId, ff.Film!.ImageUrl })
                .ToListAsync(cancellationToken);

            return lineup
                .GroupBy(ff => ff.FestivalId)
                .ToDictionary(
                    group => group.Key,
                    group => group.FirstOrDefault(entry => !string.IsNullOrWhiteSpace(entry.ImageUrl))?.ImageUrl);
        }

        /// <summary>
        /// We add a new FestivalFilm relationship to the database.
        /// </summary>
        public async Task AddAsync(FestivalFilm festivalFilm, CancellationToken cancellationToken = default)
        {
            await _dbContext.FestivalFilms.AddAsync(festivalFilm, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// We update an existing FestivalFilm record (e.g., modifying its programming notes).
        /// </summary>
        public async Task UpdateAsync(FestivalFilm festivalFilm, CancellationToken cancellationToken = default)
        {
            _dbContext.FestivalFilms.Update(festivalFilm);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// We remove a festival film link from the database.
        /// </summary>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.FestivalFilms.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                _dbContext.FestivalFilms.Remove(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
