namespace UmaFestHub.Application.Recommendations;

public sealed record FilmRecommendationDto(
    Guid FilmId,
    Guid FestivalFilmId,
    string Title,
    string? PosterUrl,
    double Score);
