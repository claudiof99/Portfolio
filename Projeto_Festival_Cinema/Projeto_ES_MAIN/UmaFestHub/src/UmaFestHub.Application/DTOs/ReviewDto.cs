namespace UmaFestHub.Application.DTOs;

// Review DTO used across the application boundary.
// Kept flat and UI-friendly (Status as string) so web/API layers don't need to reference domain entities.
public sealed record ReviewDto(
	Guid Id,
	Guid UserId,
	Guid? FestivalFilmId,
	int ExternalFilmId,
	int Rating,
	string Comment,
	string Status,
	DateTime DateUtc,
	bool IsReported,
	bool HasBeenReported,
	Guid? FilmId) : IReviewLikeDto;
