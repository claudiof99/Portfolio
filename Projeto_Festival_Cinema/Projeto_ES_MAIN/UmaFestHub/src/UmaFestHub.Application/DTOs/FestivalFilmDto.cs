namespace UmaFestHub.Application.DTOs;

public sealed record FestivalFilmDto(
    Guid Id,
    Guid FestivalId,
    Guid FilmId,
    string FilmName,
    string FilmUrl,
    string? ImageUrl,
    string? FilmDescription,
    int DurationMinutes,
    IReadOnlyList<string> Genres,
    int SessionCount,
    bool IsWorldPremier,
    IReadOnlyList<SessionDto> Sessions);