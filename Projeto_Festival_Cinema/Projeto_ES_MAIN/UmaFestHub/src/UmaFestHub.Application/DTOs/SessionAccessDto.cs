namespace UmaFestHub.Application.DTOs;
public sealed record SessionAccessDto(
    Guid UserId,
    Guid? SessionId,
    Guid FestivalId,
    Guid FestivalFilmId,
    DateTime NowUtc,
    DateTime SessionStartUtc,
    DateTime SessionEndUtc,
    DateTime FestivalEndUtc);