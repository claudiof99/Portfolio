using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces
{
    public interface IFestivalFilmService
    {
        /// <summary>
        /// Creates a FestivalFilm link between a festival and a film.
        /// </summary>
        /// <param name="festivalId">Festival Id</param>
        /// <param name="filmId">Film Id</param>
        /// <param name="isWorldPremier">Is World Premier</param>
        /// <param name="programmingNotes">Programming notes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Id of the created FestivalFilm</returns>
        
        Task<FestivalFilmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(Guid festivalId, Guid filmId, bool isWorldPremier = false, string programmingNotes = "", CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FestivalFilmDto>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default);

        Task<Guid> ImportFromTmdbAsync(Guid festivalId, int tmdbId, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<Guid, string?>> GetCoverImageUrlsByFestivalIdsAsync(
            IReadOnlyCollection<Guid> festivalIds,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid festivalFilmId, CancellationToken cancellationToken = default);
    }
}