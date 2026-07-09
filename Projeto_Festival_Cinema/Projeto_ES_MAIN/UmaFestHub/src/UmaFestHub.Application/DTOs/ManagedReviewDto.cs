namespace UmaFestHub.Application.DTOs;

// Staff-oriented review DTO for moderation screens.
// Includes author and film display fields that are derived/enriched for UI rendering.
public sealed record ManagedReviewDto(
	Guid Id,
	Guid UserId,
	string AuthorName,
	bool IsStaffAuthor,
	Guid? FestivalFilmId,
	Guid? FilmId,
	string FilmTitle,
	int ExternalFilmId,
	int Rating,
	string Comment,
	string Status,
	DateTime DateUtc,
	bool IsReported,
	bool HasBeenReported) : IManagedReviewLikeDto;

