
using UmaFestHub.Domain.Entities;
namespace UmaFestHub.Domain.Interfaces
{
    public interface IFestivalFilmRepository
    {
        Task<FestivalFilm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FestivalFilm>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default);

        /// <summary>Distinct festivals (id + name) that include at least one of the films in program.</summary>
        Task<IReadOnlyList<(Guid FestivalId, string FestivalName)>> GetDistinctFestivalsContainingFilmIdsAsync(
            IReadOnlyCollection<Guid> filmIds,
            CancellationToken cancellationToken = default);

        Task<IReadOnlySet<Guid>> GetFilmIdsInFestivalProgramAsync(Guid festivalId, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<Guid, string?>> GetFirstImportedFilmImageUrlsByFestivalIdsAsync(
            IReadOnlyCollection<Guid> festivalIds,
            CancellationToken cancellationToken = default);

        Task AddAsync(FestivalFilm festivalFilm, CancellationToken cancellationToken = default);
        Task UpdateAsync(FestivalFilm festivalFilm, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
