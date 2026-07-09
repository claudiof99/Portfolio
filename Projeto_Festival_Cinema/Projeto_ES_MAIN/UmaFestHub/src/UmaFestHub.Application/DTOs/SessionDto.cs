namespace UmaFestHub.Application.DTOs;

public sealed record SessionDto(
	Guid Id,
	Guid FestivalFilmId,
	string SessionType,
	DateTime StartTimeUtc,
	DateTime EndTimeUtc);
