namespace UmaFestHub.Application.DTOs;

// Common shape for review DTOs used by the web layer.
// Allows a single mapper to project either "public" or "managed" DTOs into a ReviewViewModel.
public interface IReviewLikeDto
{
	Guid Id { get; }
	Guid UserId { get; }
	Guid? FestivalFilmId { get; }
	Guid? FilmId { get; }
	int ExternalFilmId { get; }
	int Rating { get; }
	string Comment { get; }
	string Status { get; }
	DateTime DateUtc { get; }
	bool IsReported { get; }
	bool HasBeenReported { get; }
}

// Extra fields only available in moderation views.
public interface IManagedReviewLikeDto : IReviewLikeDto
{
	string AuthorName { get; }
	bool IsStaffAuthor { get; }
	string FilmTitle { get; }
}

