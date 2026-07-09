namespace UmaFestHub.Application.DTOs;

public sealed record FilmDto(
	Guid Id,
	int ExternalId,
	string Name,
	string Url,
	string? ImageUrl,
	string Description,
	int DurationMinutes,
	IReadOnlyList<string> Genres,
	IReadOnlyList<FilmCreditDto> Credits);
