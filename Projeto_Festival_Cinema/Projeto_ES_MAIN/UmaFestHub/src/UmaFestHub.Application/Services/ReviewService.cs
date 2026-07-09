using UmaFestHub.Application;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Application.Observers.Reviews;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

public class ReviewService : IReviewService, IReviewReplyService
{
	private readonly IReviewRepository _reviewRepository;
	private readonly IReviewReplyRepository _reviewReplyRepository;
	private readonly IUserRepository _userRepository;
	private readonly IReviewNotificationNotifier _reviewNotificationNotifier;

	public ReviewService(
		IReviewRepository reviewRepository,
		IReviewReplyRepository reviewReplyRepository,
		IUserRepository userRepository,
		IReviewNotificationNotifier reviewNotificationNotifier)
	{
		_reviewRepository = reviewRepository;
		_reviewReplyRepository = reviewReplyRepository;
		_userRepository = userRepository;
		_reviewNotificationNotifier = reviewNotificationNotifier;
	}

	public async Task<IReadOnlyList<ReviewDto>> GetForFestivalFilmAsync(Guid festivalFilmId, Guid? viewerUserId = null, CancellationToken cancellationToken = default)
	{
		var reviews = await _reviewRepository.GetForFestivalFilmAsync(festivalFilmId, viewerUserId, cancellationToken);
		return reviews.Select(Map).ToList();
	}

	public async Task<PagedResultDto<ReviewDto>> GetForFestivalFilmPageAsync(Guid festivalFilmId, Guid? viewerUserId, int page, int pageSize, CancellationToken cancellationToken = default)
	{
		// Normalize paging inputs to keep repository calls predictable and prevent negative Skip/Take.
		page = page < 1 ? 1 : page;
		pageSize = pageSize < 1 ? 9 : pageSize;

		var skip = (page - 1) * pageSize;
		// Fetch one extra item to compute HasNext without an extra COUNT(*) query.
		var take = pageSize + 1;
		var reviews = await _reviewRepository.GetForFestivalFilmPageAsync(festivalFilmId, viewerUserId, skip, take, cancellationToken);

		var hasNext = reviews.Count > pageSize;
		var pageItems = hasNext ? reviews.Take(pageSize).ToList() : reviews.ToList();

		return new PagedResultDto<ReviewDto>(
			pageItems.Select(Map).ToList(),
			page,
			hasNext);
	}

	public async Task<IReadOnlyList<ManagedReviewDto>> GetAllForManagementAsync(CancellationToken cancellationToken = default)
	{
		// Admin/Organizer management listing: includes all reviews (including hidden),
		// then enriches with author display info for UI.
		var reviews = await _reviewRepository.GetAllAsync(cancellationToken);

		// Enrich with author info for UI (staff badge + display name).
		var distinctUserIds = reviews.Select(x => x.UserId).Distinct().ToList();
		var staffByUserId = new Dictionary<Guid, bool>();
		var nameByUserId = new Dictionary<Guid, string>();
		foreach (var id in distinctUserIds)
		{
			// Avoid concurrent EF operations on the same scoped DbContext (EF Core doesn't allow parallel queries).
			var user = await _userRepository.GetByIdAsync(id, cancellationToken);
			if (user is null)
			{
				continue;
			}

			staffByUserId[user.Id] = user.Roles.Contains(UserRole.Organizer) || user.Roles.Contains(UserRole.Admin);
			nameByUserId[user.Id] = user.Name;
		}

		return reviews.Select(r =>
		{
			var isStaffAuthor = staffByUserId.TryGetValue(r.UserId, out var isStaff) && isStaff;
			var authorName = nameByUserId.TryGetValue(r.UserId, out var name) ? name : "Unknown";
			var filmTitle = r.Film?.Title ?? r.FestivalFilm?.Film?.Title ?? string.Empty;

			return new ManagedReviewDto(
				r.Id,
				r.UserId,
				authorName,
				isStaffAuthor,
				r.FestivalFilmId,
				r.FilmId,
				filmTitle,
				r.ExternalFilmId,
				r.Rating,
				r.Comment,
				r.Status.ToString(),
				r.DateUtc,
				r.IsReported,
				r.HasBeenReported);
		}).ToList();
	}

	public async Task<PagedResultDto<ManagedReviewDto>> GetAllForManagementPageAsync(
		int page,
		int pageSize,
		string? movieQuery = null,
		string? authorQuery = null,
		string? status = null,
		DateTime? dayUtc = null,
		CancellationToken cancellationToken = default)
	{
		// Normalize paging inputs to keep repository calls predictable.
		page = page < 1 ? 1 : page;
		pageSize = pageSize < 1 ? 9 : pageSize;

		var skip = (page - 1) * pageSize;
		// Fetch one extra item to compute HasNext without a COUNT(*) query.
		var take = pageSize + 1;
		IReadOnlyList<Guid>? userIds = null;
		if (!string.IsNullOrWhiteSpace(authorQuery))
		{
			// Author filter is name-based in the UI; convert it to user IDs once,
			// so the reviews query can stay set-based.
			userIds = await _userRepository.SearchIdsByNameAsync(authorQuery, cancellationToken);
			if (userIds.Count == 0)
			{
				// Short-circuit: no users match the author query, so no reviews can match.
				return new PagedResultDto<ManagedReviewDto>(Array.Empty<ManagedReviewDto>(), page, HasNext: false);
			}
		}

		var reviews = await _reviewRepository.GetAllFilteredPageAsync(movieQuery, status, dayUtc, userIds, skip, take, cancellationToken);

		var hasNext = reviews.Count > pageSize;
		var pageItems = hasNext ? reviews.Take(pageSize).ToList() : reviews.ToList();

		// Enrich with author info for UI (staff badge + display name).
		var distinctUserIds = pageItems.Select(x => x.UserId).Distinct().ToList();
		var staffByUserId = new Dictionary<Guid, bool>();
		var nameByUserId = new Dictionary<Guid, string>();
		foreach (var id in distinctUserIds)
		{
			// Avoid concurrent EF operations on the same scoped DbContext (EF Core doesn't allow parallel queries).
			var user = await _userRepository.GetByIdAsync(id, cancellationToken);
			if (user is null)
			{
				continue;
			}

			staffByUserId[user.Id] = user.Roles.Contains(UserRole.Organizer) || user.Roles.Contains(UserRole.Admin);
			nameByUserId[user.Id] = user.Name;
		}

		var dtos = pageItems.Select(r =>
		{
			var isStaffAuthor = staffByUserId.TryGetValue(r.UserId, out var isStaff) && isStaff;
			var authorName = nameByUserId.TryGetValue(r.UserId, out var name) ? name : "Unknown";
			var filmTitle = r.Film?.Title ?? r.FestivalFilm?.Film?.Title ?? string.Empty;

			return new ManagedReviewDto(
				r.Id,
				r.UserId,
				authorName,
				isStaffAuthor,
				r.FestivalFilmId,
				r.FilmId,
				filmTitle,
				r.ExternalFilmId,
				r.Rating,
				r.Comment,
				r.Status.ToString(),
				r.DateUtc,
				r.IsReported,
				r.HasBeenReported);
		}).ToList();

		return new PagedResultDto<ManagedReviewDto>(dtos, page, hasNext);
	}

	public async Task<Guid> AddAsync(ReviewDto review, CancellationToken cancellationToken = default)
	{
		// Domain owns invariants (rating bounds, comment required/length, etc.).
		var parsedStatus = Enum.TryParse<ReviewStatus>(review.Status, ignoreCase: true, out var status)
			? status
			: ReviewStatus.Pending;

		var entity = Review.Create(
			userId: review.UserId,
			festivalFilmId: review.FestivalFilmId,
			filmId: review.FilmId,
			externalFilmId: review.ExternalFilmId,
			rating: review.Rating,
			comment: review.Comment,
			status: parsedStatus,
			dateUtc: DateTime.UtcNow);

		await _reviewRepository.AddAsync(entity, cancellationToken);

		if (entity.Status == ReviewStatus.Pending)
		{
			await _reviewNotificationNotifier.NotifyReviewPendingModerationAsync(
				new ReviewPendingModerationContext(entity.Id, entity.FestivalFilmId),
				cancellationToken);
		}

		return entity.Id;
	}

	public async Task ReportAsync(Guid reviewId, CancellationToken cancellationToken = default)
	{
		// Reporting pushes a review back into moderation flow:
		// mark reported, remember that it was ever reported, and reset status to Pending.
		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
		if (review is null)
		{
			return;
		}

		review.IsReported = true;
		review.HasBeenReported = true;
		review.Status = ReviewStatus.Pending;
		await _reviewRepository.UpdateAsync(review, cancellationToken);
	}

	public async Task HideReportedAsync(Guid reviewId, CancellationToken cancellationToken = default)
	{
		// "Hide" removes the review from public listings (hard filter on IsHiddenByAdmin)
		// and marks it as rejected for management visibility.
		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
		if (review is null)
		{
			return;
		}

		if (review.IsHiddenByAdmin && review.Status == ReviewStatus.Rejected)
		{
			return;
		}

		review.IsHiddenByAdmin = true;
		review.Status = ReviewStatus.Rejected;
		await _reviewRepository.UpdateAsync(review, cancellationToken);

		var enriched = await _reviewRepository.GetByIdWithFestivalAndFilmAsync(reviewId, cancellationToken);
		var forNotify = enriched ?? review;
		await _reviewNotificationNotifier.NotifyReviewAuthorOutcomeAsync(
			BuildAuthorOutcome(forNotify, isApproved: false),
			cancellationToken);
	}

	public async Task ApproveAsync(Guid reviewId, CancellationToken cancellationToken = default)
	{
		// "Approve" makes the review public again and clears the reported flag,
		// but keeps HasBeenReported=true to enforce "report-lock" after staff approval.
		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
		if (review is null)
		{
			return;
		}

		var wasAlreadyApproved = review.Status == ReviewStatus.Approved;

		review.Status = ReviewStatus.Approved;
		review.IsReported = false;
		// Keep HasBeenReported=true so approved-after-report cannot be reported again.
		review.IsHiddenByAdmin = false;
		await _reviewRepository.UpdateAsync(review, cancellationToken);

		if (!wasAlreadyApproved)
		{
			var enriched = await _reviewRepository.GetByIdWithFestivalAndFilmAsync(reviewId, cancellationToken);
			var forNotify = enriched ?? review;
			await _reviewNotificationNotifier.NotifyReviewAuthorOutcomeAsync(
				BuildAuthorOutcome(forNotify, isApproved: true),
				cancellationToken);
		}
	}

	public async Task<IReadOnlyList<ReviewReplyDto>> GetRepliesByReviewIdsAsync(
		IReadOnlyList<Guid> reviewIds,
		Guid? viewerUserId,
		bool isModerator,
		CancellationToken cancellationToken = default)
	{
		var rows = await _reviewReplyRepository.GetForReviewIdsAsync(reviewIds, viewerUserId, cancellationToken);
		return rows
			.Select(MapReply)
			.Where(d => ReviewReplyVisibility.CanSee(d, viewerUserId, isModerator))
			.ToList();
	}

	public async Task<IReadOnlyList<ReviewReplyDto>> GetRepliesByReviewIdsForManagementAsync(
		IReadOnlyList<Guid> reviewIds,
		CancellationToken cancellationToken = default)
	{
		var rows = await _reviewReplyRepository.GetForReviewIdsForManagementAsync(reviewIds, cancellationToken);
		return rows.Select(MapReply).ToList();
	}

	public async Task<IReadOnlyList<ReviewReplyDto>> GetRepliesForReviewAsync(
		Guid festivalFilmId,
		Guid reviewId,
		Guid? viewerUserId,
		bool isModerator,
		CancellationToken cancellationToken = default)
	{
		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
		if (review is null || review.FestivalFilmId != festivalFilmId)
		{
			return Array.Empty<ReviewReplyDto>();
		}

		var rows = await _reviewReplyRepository.GetForReviewIdAsync(reviewId, viewerUserId, cancellationToken);
		return rows
			.Select(MapReply)
			.Where(d => ReviewReplyVisibility.CanSee(d, viewerUserId, isModerator))
			.ToList();
	}

	public async Task<(Guid? ReplyId, UserMessage? Error)> AddReplyAsync(
		Guid userId,
		Guid festivalFilmId,
		Guid reviewId,
		string comment,
		ReviewStatus initialStatus,
		CancellationToken cancellationToken = default)
	{
		if (festivalFilmId == Guid.Empty)
		{
			return (null, new UserMessage(UserMessageKeys.Review_InvalidFestivalFilm));
		}

		if (reviewId == Guid.Empty)
		{
			return (null, new UserMessage(UserMessageKeys.Review_InvalidReviewId));
		}

		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
		if (review is null || review.FestivalFilmId != festivalFilmId)
		{
			return (null, new UserMessage(UserMessageKeys.Review_NotFoundForTitle));
		}

		try
		{
			var entity = ReviewReply.Create(userId, reviewId, comment, initialStatus);
			await _reviewReplyRepository.AddAsync(entity, cancellationToken);

			if (entity.Status == ReviewStatus.Pending)
			{
				await _reviewNotificationNotifier.NotifyReplyPendingModerationAsync(
					new ReplyPendingModerationContext(entity.Id, reviewId, review.FestivalFilmId),
					cancellationToken);
			}

			return (entity.Id, null);
		}
		catch (ArgumentException ex) when (ex.ParamName == nameof(comment))
		{
			return ex.Message.Contains("required", StringComparison.OrdinalIgnoreCase)
				? (null, new UserMessage("Review_CommentRequired"))
				: (null, new UserMessage("Review_CommentMaxLength"));
		}
	}

	public async Task ReportReplyAsync(Guid replyId, CancellationToken cancellationToken = default)
	{
		var reply = await _reviewReplyRepository.GetByIdAsync(replyId, cancellationToken);
		if (reply is null)
		{
			return;
		}

		reply.IsReported = true;
		reply.HasBeenReported = true;
		reply.Status = ReviewStatus.Pending;
		await _reviewReplyRepository.UpdateAsync(reply, cancellationToken);
	}

	public async Task HideReplyAsync(Guid replyId, CancellationToken cancellationToken = default)
	{
		var reply = await _reviewReplyRepository.GetByIdAsync(replyId, cancellationToken);
		if (reply is null)
		{
			return;
		}

		if (reply.IsHiddenByAdmin && reply.Status == ReviewStatus.Rejected)
		{
			return;
		}

		reply.IsHiddenByAdmin = true;
		reply.Status = ReviewStatus.Rejected;
		await _reviewReplyRepository.UpdateAsync(reply, cancellationToken);

		var enriched = await _reviewReplyRepository.GetByIdWithReviewFestivalAndFilmAsync(replyId, cancellationToken);
		var forNotify = enriched ?? reply;
		await _reviewNotificationNotifier.NotifyReplyAuthorOutcomeAsync(
			BuildReplyAuthorOutcome(forNotify, isApproved: false),
			cancellationToken);
	}

	public async Task ApproveReplyAsync(Guid replyId, CancellationToken cancellationToken = default)
	{
		var reply = await _reviewReplyRepository.GetByIdAsync(replyId, cancellationToken);
		if (reply is null)
		{
			return;
		}

		var wasAlreadyApproved = reply.Status == ReviewStatus.Approved;

		reply.Status = ReviewStatus.Approved;
		reply.IsReported = false;
		reply.IsHiddenByAdmin = false;
		await _reviewReplyRepository.UpdateAsync(reply, cancellationToken);

		if (!wasAlreadyApproved)
		{
			var enriched = await _reviewReplyRepository.GetByIdWithReviewFestivalAndFilmAsync(replyId, cancellationToken);
			var forNotify = enriched ?? reply;
			await _reviewNotificationNotifier.NotifyReplyAuthorOutcomeAsync(
				BuildReplyAuthorOutcome(forNotify, isApproved: true),
				cancellationToken);
		}
	}

	public async Task<ReviewReplyDto?> GetReplyByIdAsync(Guid replyId, CancellationToken cancellationToken = default)
	{
		var reply = await _reviewReplyRepository.GetByIdAsync(replyId, cancellationToken);
		return reply is null ? null : MapReply(reply);
	}

	private static ReviewAuthorOutcomeContext BuildAuthorOutcome(Review review, bool isApproved)
	{
		var filmTitle = review.FestivalFilm?.Film?.Name ?? review.Film?.Name;
		if (string.IsNullOrWhiteSpace(filmTitle))
		{
			filmTitle = "—";
		}
		else
		{
			filmTitle = filmTitle.Trim();
		}

		var festivalName = review.FestivalFilm?.Festival?.Name;
		if (string.IsNullOrWhiteSpace(festivalName))
		{
			festivalName = "—";
		}
		else
		{
			festivalName = festivalName.Trim();
		}

		return new ReviewAuthorOutcomeContext(
			review.Id,
			review.UserId,
			isApproved,
			review.Rating,
			review.Comment ?? string.Empty,
			filmTitle,
			festivalName);
	}

	private static ReplyAuthorOutcomeContext BuildReplyAuthorOutcome(ReviewReply reply, bool isApproved)
	{
		var review = reply.Review;
		var filmTitle = review?.FestivalFilm?.Film?.Name ?? review?.Film?.Name;
		if (string.IsNullOrWhiteSpace(filmTitle))
		{
			filmTitle = "—";
		}
		else
		{
			filmTitle = filmTitle.Trim();
		}

		var festivalName = review?.FestivalFilm?.Festival?.Name;
		if (string.IsNullOrWhiteSpace(festivalName))
		{
			festivalName = "—";
		}
		else
		{
			festivalName = festivalName.Trim();
		}

		return new ReplyAuthorOutcomeContext(
			reply.Id,
			reply.UserId,
			isApproved,
			reply.Comment ?? string.Empty,
			filmTitle,
			festivalName);
	}

	private static ReviewReplyDto MapReply(ReviewReply r) =>
		new(
			r.Id,
			r.ReviewId,
			r.UserId,
			r.Comment,
			r.DateUtc,
			r.Status.ToString(),
			r.IsReported,
			r.HasBeenReported,
			r.IsHiddenByAdmin);

	private static ReviewDto Map(Review review) =>
		new(
			review.Id,
			review.UserId,
			review.FestivalFilmId,
			review.ExternalFilmId,
			review.Rating,
			review.Comment,
			review.Status.ToString(),
			review.DateUtc,
			review.IsReported,
			review.HasBeenReported,
			review.FilmId);
}
