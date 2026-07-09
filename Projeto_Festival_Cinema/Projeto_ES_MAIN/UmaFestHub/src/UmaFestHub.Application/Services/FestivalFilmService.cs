using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services
{
    /// <summary>
    /// We use this service to manage the relationship between festivals and films, acting as the orchestrator for importing and scheduling.
    /// </summary>
    public class FestivalFilmService : IFestivalFilmService
    {
        private readonly IFestivalFilmRepository _festivalFilmRepository;
        private readonly IFilmService _filmService;
        private readonly IProductRepository _productRepository;

        public FestivalFilmService(IFestivalFilmRepository festivalFilmRepository, IFilmService filmService, IProductRepository productRepository)
        {
            _festivalFilmRepository = festivalFilmRepository;
            _filmService = filmService;
            _productRepository = productRepository;
        }

        public async Task<FestivalFilmDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return null;

            var festivalFilm = await _festivalFilmRepository.GetByIdAsync(id, cancellationToken);
            return festivalFilm is null ? null : Map(festivalFilm);
        }

        /// <summary>
        /// We create a new link between an existing film and a festival, saving any programming notes or premiere status.
        /// </summary>
        public async Task<Guid> CreateAsync(Guid festivalId, Guid filmId, bool isWorldPremier = false, string programmingNotes = "", CancellationToken cancellationToken = default)
        {
            var entity = new FestivalFilm
            {
                Id = Guid.NewGuid(),
                FestivalId = festivalId,
                FilmId = filmId,
                IsWorldPremier = isWorldPremier,
                ProgrammingNotes = programmingNotes ?? string.Empty,
                AddedAtUtc = DateTime.UtcNow
            };
            await _festivalFilmRepository.AddAsync(entity, cancellationToken);

            var rental = new Rental(
                festivalFilmId: entity.Id,
                price: 4.99m,
                duration: new Duration(48, DurationUnit.Hours)
            );
            await _productRepository.AddAsync(rental, cancellationToken);
            
            return entity.Id;
        }

        /// <summary>
        /// We fetch all films that are currently scheduled for a specific festival and map them to our Data Transfer Objects.
        /// </summary>
        public async Task<IReadOnlyList<FestivalFilmDto>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default)
        {
            var items = await _festivalFilmRepository.GetByFestivalIdAsync(festivalId, cancellationToken);
            return items.Select(ff => new FestivalFilmDto(
                ff.Id,
                ff.FestivalId,
                ff.FilmId,
                ff.Film?.Name ?? "Unknown",
                ff.Film?.Url ?? string.Empty,
                ff.Film?.ImageUrl,
                ff.Film?.Description,
                ff.Film?.Duration != null ? ff.Film.Duration.ToMinutes() : 0,
                ff.Film?.Genres.Select(g => g.Name).ToList() ?? new List<string>(),
                ff.Sessions?.Count ?? 0,
                ff.IsWorldPremier,
                ff.Sessions?.Select(s => new SessionDto(
                    s.Id,
                    s.FestivalFilmId,
                    s.SessionType,
                    s.StartTimeUtc,
                    s.EndTimeUtc)).ToList() ?? [])).ToList();
        }

        /// <summary>
        /// We import a film directly from TMDb. We first create the film in our database (which handles deduplication) and then link it to the festival.
        /// </summary>
        public async Task<Guid> ImportFromTmdbAsync(Guid festivalId, int tmdbId, CancellationToken cancellationToken = default)
        {
            var filmDto = new FilmDto(Guid.Empty, tmdbId, string.Empty, string.Empty, null, string.Empty, 0, new List<string>(), new List<FilmCreditDto>());
            var filmId = await _filmService.CreateAsync(filmDto, cancellationToken);

            var existing = await _festivalFilmRepository.GetByFestivalIdAsync(festivalId, cancellationToken);
            var existingLink = existing.FirstOrDefault(ff => ff.FilmId == filmId);
            if (existingLink != null)
            {
                return existingLink.Id;
            }

            return await CreateAsync(festivalId, filmId, false, string.Empty, cancellationToken);
        }

        public Task<IReadOnlyDictionary<Guid, string?>> GetCoverImageUrlsByFestivalIdsAsync(
            IReadOnlyCollection<Guid> festivalIds,
            CancellationToken cancellationToken = default)
            => _festivalFilmRepository.GetFirstImportedFilmImageUrlsByFestivalIdsAsync(festivalIds, cancellationToken);

        /// <summary>
        /// We safely remove a scheduled film from a festival.
        /// </summary>
        public async Task DeleteAsync(Guid festivalFilmId, CancellationToken cancellationToken = default)
        {
            var festivalFilm = await _festivalFilmRepository.GetByIdAsync(festivalFilmId, cancellationToken);
            if (festivalFilm is null)
                return;

            await _festivalFilmRepository.DeleteAsync(festivalFilmId, cancellationToken);
        }

        private static FestivalFilmDto Map(FestivalFilm ff) =>
        new(
            ff.Id,
            ff.FestivalId,
            ff.FilmId,
            ff.Film?.Name ?? "Unknown",
            ff.Film?.Url ?? string.Empty,
            ff.Film?.ImageUrl,
            ff.Film?.Description,
            ff.Film?.Duration != null ? ff.Film.Duration.ToMinutes() : 0,
            ff.Film?.Genres.Select(g => g.Name).ToList() ?? [],
            ff.Sessions?.Count ?? 0,
            ff.IsWorldPremier,
            ff.Sessions?.Select(s => new SessionDto(
                s.Id,
                s.FestivalFilmId,
                s.SessionType,
                s.StartTimeUtc,
                s.EndTimeUtc)).ToList() ?? []);
        }
}
