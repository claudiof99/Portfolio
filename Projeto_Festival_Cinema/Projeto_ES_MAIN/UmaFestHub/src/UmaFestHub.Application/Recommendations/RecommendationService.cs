using Microsoft.Extensions.Logging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Recommendations;

public sealed class RecommendationService : IRecommendationService
{
    private const double WeightQuality    = 0.35;
    private const double WeightSocial     = 0.30;
    private const double WeightAwards     = 0.10;
    private const double WeightGenre       = 0.15;
    private const double WeightPopularity = 0.10;

    private readonly IFestivalRepository _festivalRepository;
    private readonly IFestivalFilmRepository _festivalFilmRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IAwardRepository _awardRepository;
    private readonly IPersonalListRepository _personalListRepository;
    private readonly ILogger<RecommendationService> _logger;

    public RecommendationService(
        IFestivalRepository festivalRepository,
        IFestivalFilmRepository festivalFilmRepository,
        IReviewRepository reviewRepository,
        IVoteRepository voteRepository,
        IAwardRepository awardRepository,
        IPersonalListRepository personalListRepository,
        ILogger<RecommendationService> logger)
    {
        _festivalRepository = festivalRepository;
        _festivalFilmRepository = festivalFilmRepository;
        _reviewRepository = reviewRepository;
        _voteRepository = voteRepository;
        _awardRepository = awardRepository;
        _personalListRepository = personalListRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FilmRecommendationDto>> GetAsync(
        Guid userId, Guid festivalId, int maxResults = 6, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetAsync called: userId={UserId}, festivalId={FestivalId}", userId, festivalId);

        var festivalFilms = await _festivalFilmRepository
            .GetByFestivalIdAsync(festivalId, cancellationToken);

        _logger.LogInformation("Found {Count} festival films", festivalFilms.Count);

        if (festivalFilms.Count == 0)
            return Array.Empty<FilmRecommendationDto>();

        var filmIds = festivalFilms.Select(ff => ff.FilmId).ToList();

        // Load user's personal lists to get preferred genres
        var allEntries = new List<PersonalList>();

        foreach (var listType in new[]
        {
            PersonalListType.Favorites,
            PersonalListType.Watched,
            PersonalListType.Watchlist
        })
        {
            var entries = await _personalListRepository
                .GetFullByUserAndTypeAsync(userId, listType, cancellationToken);
            allEntries.AddRange(entries);
        }

        // Derive preferred genres from personal lists
        var preferredGenres = allEntries
            .SelectMany(e => e.Film != null
                ? e.Film.Genres.Select(g => g.Name)
                : Enumerable.Empty<string>())
            .GroupBy(g => g, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _logger.LogInformation("Preferred genres: {Genres}", string.Join(", ", preferredGenres));

        // Load signal data sequentially to avoid DbContext concurrency issues
        var allReviews = await _reviewRepository.GetApprovedForFilmsAsync(filmIds, cancellationToken);
        _logger.LogInformation("Got {Count} reviews", allReviews.Count);

        var voteCounts = await _voteRepository.GetVoteCountsByFilmIdsAsync(festivalId, filmIds, cancellationToken);
        _logger.LogInformation("Got {Count} vote counts", voteCounts?.Count ?? 0);

        var nominations = await _awardRepository.GetNominationCountsByFilmIdsAsync(festivalId, filmIds, cancellationToken);
        _logger.LogInformation("Got {Count} nominations", nominations?.Count ?? 0);

        voteCounts ??= new Dictionary<Guid, int>();
        nominations ??= new Dictionary<Guid, int>();

        // Exclude the current user's reviews to avoid self-bias
        var externalReviews = allReviews.Where(r => r.UserId != userId).ToList();
        var reviews = externalReviews.ToLookup(r => r.FilmId ?? Guid.Empty);

        var candidates = festivalFilms
            .Select(ff => new
            {
                FestivalFilm = ff,
                Score = ComputeScore(ff, reviews, voteCounts, nominations, preferredGenres)
            })
            .ToList();

        // If all scores are 0, use popularity as a base and boost by genre match
        if (candidates.All(x => x.Score == 0))
        {
            _logger.LogInformation("Using popularity + genre fallback");
            candidates = festivalFilms
                .Select(ff => new
                {
                    FestivalFilm = ff,
                    Score = PopularityScore(ff.Film) * 50 + GenreScore(ff.Film, preferredGenres) * 50
                })
                .ToList();
        }

        return candidates
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.FestivalFilm.FilmId)
            .Take(maxResults)
            .Select(x => new FilmRecommendationDto(
                x.FestivalFilm.FilmId,
                x.FestivalFilm.Id,
                x.FestivalFilm.Film?.Name ?? "Unknown",
                x.FestivalFilm.Film?.ImageUrl,
                Math.Round(x.Score, 4)))
            .ToList();
    }

    public async Task<IReadOnlyList<FilmRecommendationDto>> GetFromAllFestivalsAsync(
        Guid userId, int maxResults = 6, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetFromAllFestivalsAsync called: userId={UserId}", userId);

        var festivals = await _festivalRepository.GetAllAsync(cancellationToken);
        var now = DateTime.UtcNow;

        // Get active or upcoming festivals (those with films available)
        var activeFestivals = festivals
            .Where(f => f.StartDateUtc <= now && f.EndDateUtc >= now || f.StartDateUtc > now)
            .ToList();

        _logger.LogInformation("Found {Count} active/upcoming festivals", activeFestivals.Count);

        if (activeFestivals.Count == 0)
            return Array.Empty<FilmRecommendationDto>();

        // Load all festival films from all festivals
        var allFestivalFilms = new List<FestivalFilm>();
        var filmIdsByFestival = new Dictionary<Guid, List<Guid>>();

        foreach (var festival in activeFestivals)
        {
            var festivalFilms = await _festivalFilmRepository.GetByFestivalIdAsync(festival.Id, cancellationToken);
            allFestivalFilms.AddRange(festivalFilms);
            filmIdsByFestival[festival.Id] = festivalFilms.Select(ff => ff.FilmId).ToList();
        }

        _logger.LogInformation("Found {Count} total festival films", allFestivalFilms.Count);

        if (allFestivalFilms.Count == 0)
            return Array.Empty<FilmRecommendationDto>();

        // Load user's personal lists to get preferred genres
        var allEntries = new List<PersonalList>();
        foreach (var listType in new[]
        {
            PersonalListType.Favorites,
            PersonalListType.Watched,
            PersonalListType.Watchlist
        })
        {
            var entries = await _personalListRepository
                .GetFullByUserAndTypeAsync(userId, listType, cancellationToken);
            allEntries.AddRange(entries);
        }

        // Derive preferred genres from personal lists
        var preferredGenres = allEntries
            .SelectMany(e => e.Film != null
                ? e.Film.Genres.Select(g => g.Name)
                : Enumerable.Empty<string>())
            .GroupBy(g => g, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Load all reviews across all festival films
        var allFilmIds = allFestivalFilms.Select(ff => ff.FilmId).ToList();
        var allReviews = await _reviewRepository.GetApprovedForFilmsAsync(allFilmIds, cancellationToken);

        // Load votes and nominations per festival
        var votesByFestival = new Dictionary<Guid, IReadOnlyDictionary<Guid, int>>();
        var nominationsByFestival = new Dictionary<Guid, IReadOnlyDictionary<Guid, int>>();

        foreach (var festival in activeFestivals)
        {
            var filmIds = filmIdsByFestival[festival.Id];
            votesByFestival[festival.Id] = await _voteRepository.GetVoteCountsByFilmIdsAsync(festival.Id, filmIds, cancellationToken)
                ?? new Dictionary<Guid, int>();
            nominationsByFestival[festival.Id] = await _awardRepository.GetNominationCountsByFilmIdsAsync(festival.Id, filmIds, cancellationToken)
                ?? new Dictionary<Guid, int>();
        }

        // Exclude current user's reviews
        var externalReviews = allReviews.Where(r => r.UserId != userId).ToList();
        var reviews = externalReviews.ToLookup(r => r.FilmId ?? Guid.Empty);

        var candidates = new List<ScoredFilm>();
        foreach (var festival in activeFestivals)
        {
            var festivalFilms = allFestivalFilms.Where(ff => ff.FestivalId == festival.Id).ToList();
            var voteCounts = votesByFestival[festival.Id];
            var nominations = nominationsByFestival[festival.Id];

            foreach (var ff in festivalFilms)
            {
                var score = ComputeScore(ff, reviews, voteCounts, nominations, preferredGenres);
                candidates.Add(new ScoredFilm(ff, score, festival.Name));
            }
        }

        // Fallback: if all scores are 0, use popularity + genre
        if (candidates.All(x => x.Score == 0))
        {
            _logger.LogInformation("All scores are 0, using popularity + genre fallback");
            candidates = allFestivalFilms
                .Select(ff => new ScoredFilm(ff,
                    PopularityScore(ff.Film) * 50 + GenreScore(ff.Film, preferredGenres) * 50,
                    ff.Festival?.Name ?? "Unknown"))
                .ToList();
        }

        // If STILL all 0 (no reviews, no popularity data, no genre preferences), show all films equally
        if (candidates.All(x => x.Score == 0))
        {
            _logger.LogInformation("All scores still 0, showing all films with equal weight");
            candidates = allFestivalFilms
                .Select((ff, idx) => new ScoredFilm(ff, 50.0 - idx, ff.Festival?.Name ?? "Unknown"))
                .ToList();
        }

        // Deduplicate by FilmId - keep the one with highest score
        var deduplicated = candidates
            .GroupBy(x => x.FestivalFilm.FilmId)
            .Select(g => g.OrderByDescending(x => x.Score).First())
            .OrderByDescending(x => x.Score)
            .Take(maxResults)
            .Select(x => new FilmRecommendationDto(
                x.FestivalFilm.FilmId,
                x.FestivalFilm.Id,
                x.FestivalFilm.Film?.Name ?? "Unknown",
                x.FestivalFilm.Film?.ImageUrl,
                Math.Round(x.Score, 4)))
            .ToList();

        _logger.LogInformation("Returning {Count} deduplicated recommendations", deduplicated.Count);
        return deduplicated;
    }

    private record ScoredFilm(FestivalFilm FestivalFilm, double Score, string FestivalName);

    private double ComputeScore(
        FestivalFilm ff,
        ILookup<Guid, Review> reviews,
        IReadOnlyDictionary<Guid, int> voteCounts,
        IReadOnlyDictionary<Guid, int> nominations,
        HashSet<string> preferredGenres)
    {
        return
            WeightQuality    * QualityScore(ff.FilmId, reviews)         +
            WeightSocial     * SocialScore(ff.FilmId, voteCounts)       +
            WeightAwards     * NominationScore(ff.FilmId, nominations)  +
            WeightGenre      * GenreScore(ff.Film, preferredGenres)    +
            WeightPopularity * PopularityScore(ff.Film);
    }

    private static double QualityScore(Guid filmId, ILookup<Guid, Review> reviews)
    {
        var list = reviews[filmId].ToList();
        if (list.Count == 0) return 0;
        return (list.Average(r => r.Rating) - 1.0) / 4.0;
    }

    private static double SocialScore(Guid filmId, IReadOnlyDictionary<Guid, int> voteCounts)
    {
        if (!voteCounts.TryGetValue(filmId, out var votes) || votes == 0)
            return 0;
        var max = voteCounts.Values.Max();
        return max > 0 ? (double)votes / max : 0;
    }

    private static double NominationScore(Guid filmId, IReadOnlyDictionary<Guid, int> nominations)
    {
        if (!nominations.TryGetValue(filmId, out var count) || count == 0)
            return 0;
        var max = nominations.Values.Max();
        return max > 0 ? (double)count / max : 0;
    }

    private static double GenreScore(Film? film, HashSet<string> preferredGenres)
    {
        if (preferredGenres.Count == 0 || film == null) return 0;
        var matches = film.Genres.Count(g => preferredGenres.Contains(g.Name));
        return matches > 0 ? Math.Min(matches / 3.0, 1.0) : 0;
    }

    private static double PopularityScore(Film? film)
        => film != null && film.TmdbPopularity > 0
            ? Math.Min((double)film.TmdbPopularity / 1000.0, 1.0)
            : 0;
}
