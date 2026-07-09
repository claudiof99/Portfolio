// In-app notifications: immutable payloads passed from ReviewService into review notification observers.
namespace UmaFestHub.Application.Observers.Reviews;

public sealed record ReviewPendingModerationContext(Guid ReviewId, Guid? FestivalFilmId);

public sealed record ReplyPendingModerationContext(Guid ReplyId, Guid ReviewId, Guid? FestivalFilmId);

/// <summary>Author-facing moderation outcome (shown in the in-app notification modal).</summary>
public sealed record ReviewAuthorOutcomeContext(
	Guid ReviewId,
	Guid AuthorUserId,
	bool IsApproved,
	int Rating,
	string ReviewComment,
	string FilmTitle,
	string FestivalName);

/// <summary>Reply author-facing moderation outcome (shown in the in-app notification modal).</summary>
public sealed record ReplyAuthorOutcomeContext(
	Guid ReplyId,
	Guid AuthorUserId,
	bool IsApproved,
	string ReplyComment,
	string FilmTitle,
	string FestivalName);
