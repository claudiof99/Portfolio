namespace UmaFestHub.Application.DTOs;

public sealed record ExternalFilmMetadataDto(
	int ExternalId,
	string Title,
	string Synopsis,
	IReadOnlyList<string> Genres,
	int DurationMinutes,
	IReadOnlyList<FilmCreditDto> Credits,
	string ViewingUrl,
	string? PosterUrl = null,
	double Popularity = 0.0);

public sealed record FilmCreditDto(string Role, string PersonName, string? ImageUrl = null);

public sealed record PaymentResultDto(bool IsSuccessful, string TransactionId, string? ErrorMessage);
