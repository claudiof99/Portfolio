// -----------------------------------------------------------------------------
// Reviews UI — film review pages + staff Manage. Reply actions use IReviewReplyService;
// review listing/moderation uses IReviewService.
// -----------------------------------------------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.Security;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

public class ReviewController : Controller
{
	private readonly IReviewService _reviewService;
	private readonly IReviewReplyService _reviewReplyService;
	private readonly IFestivalFilmRepository _festivalFilmRepository;
	private readonly IUserRepository _userRepository;
	private readonly IReviewRepository _reviewRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public ReviewController(
		IReviewService reviewService,
		IReviewReplyService reviewReplyService,
		IFestivalFilmRepository festivalFilmRepository,
		IUserRepository userRepository,
		IReviewRepository reviewRepository,
		IStringLocalizer<SharedResources> localizer)
	{
		_reviewService = reviewService;
		_reviewReplyService = reviewReplyService;
		_festivalFilmRepository = festivalFilmRepository;
		_userRepository = userRepository;
		_reviewRepository = reviewRepository;
		_localizer = localizer;
	}

	[AllowAnonymous]
	[HttpGet("/review", Name = "ReviewIndex")]
	public async Task<IActionResult> Index(Guid festivalFilmId, int page = 1, CancellationToken cancellationToken = default)
	{
		// Public reviews page for a single festival-film (not a general "all reviews" listing).
		if (festivalFilmId == Guid.Empty)
		{
			return View(new ReviewIndexPageViewModel { FestivalFilmId = festivalFilmId, Page = 1, Reviews = Array.Empty<ReviewViewModel>() });
		}

		// UI-only enrichment: the list wants to show the film title as a heading.
		var festivalFilm = await _festivalFilmRepository.GetByIdAsync(festivalFilmId, cancellationToken);
		var filmTitle = festivalFilm?.Film?.Title ?? festivalFilm?.Film?.Name ?? string.Empty;
		var filmImageUrl = festivalFilm?.Film?.ImageUrl;

		// Optional viewer user id (used to include the viewer's own hidden/rejected reviews).
		var viewerUserId = Guid.TryParse(User.FindFirst("sub")?.Value, out var parsedViewerId)
			? parsedViewerId
			: (Guid?)null;

		// 9 per page to match the 3x3 card grid.
		var pageSize = 9;
		// Include hidden/rejected reviews only for the current author so they can see moderation outcomes,
		// while keeping them hidden for everyone else.
		var result = await _reviewService.GetForFestivalFilmPageAsync(festivalFilmId, viewerUserId, page, pageSize, cancellationToken);
		var viewModels = result.Items.Select(x => x.ToViewModel()).ToList();

		var isModerator = User.IsInAnyRole(RoleConstants.ModeratorRoles);
		var replyDtos = await _reviewReplyService.GetRepliesByReviewIdsAsync(viewModels.Select(x => x.Id).ToList(), viewerUserId, isModerator, cancellationToken);
		var repliesByReviewId = replyDtos.GroupBy(x => x.ReviewId).ToDictionary(g => g.Key, g => g.ToList());

		// Enrich with author info for UI (staff badge + display name).
		var distinctUserIds = viewModels
			.Select(x => x.UserId)
			.Concat(replyDtos.Select(x => x.UserId))
			.Distinct()
			.ToList();
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

			// "Staff author" means the review was written by Organizer/Admin; customers shouldn't be able to report staff content.
			staffByUserId[user.Id] = user.Roles.Contains(UserRole.Organizer) || user.Roles.Contains(UserRole.Admin);
			nameByUserId[user.Id] = user.Name;
		}

		foreach (var vm in viewModels)
		{
			vm.IsStaffAuthor = staffByUserId.TryGetValue(vm.UserId, out var isStaff) && isStaff;
			vm.AuthorName = nameByUserId.TryGetValue(vm.UserId, out var name) ? name : _localizer["Common_Unknown"].Value;

			if (repliesByReviewId.TryGetValue(vm.Id, out var repliesForReview))
			{
				vm.Replies = repliesForReview.Select(r => r.ToViewModel()).ToList();
				foreach (var replyVm in vm.Replies)
				{
					replyVm.AuthorName = nameByUserId.TryGetValue(replyVm.UserId, out var replyName) ? replyName : _localizer["Common_Unknown"].Value;
					replyVm.IsStaffAuthor = staffByUserId.TryGetValue(replyVm.UserId, out var replyStaff) && replyStaff;
				}
			}
		}

		return View(new ReviewIndexPageViewModel
		{
			FestivalFilmId = festivalFilmId,
			FilmTitle = filmTitle,
			FilmImageUrl = filmImageUrl,
			Page = result.Page,
			HasNext = result.HasNext,
			Reviews = viewModels
		});
	}

	[Authorize]
	[HttpPost("/Review/Add")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Add(CreateReviewInputModel input, CancellationToken cancellationToken)
	{
		// Review creation is tied to the authenticated user; never accept UserId from the client.
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		string RedirectToReviewPage(Guid festivalFilmId)
			// Keep the user on the form area after POST (anchor lives on the submit button).
			=> $"/Review?festivalFilmId={festivalFilmId}#submit-review";

		if (input.FestivalFilmId == Guid.Empty)
		{
			return Redirect(RedirectToReviewPage(input.FestivalFilmId));
		}

		// Server-side validation (do not rely on client-side HTML constraints).
		var comment = (input.Comment ?? string.Empty).Trim();
		if (input.Rating is < 1 or > 5)
		{
			TempData["ReviewError"] = _localizer["Review_RatingRange"].Value;
			return Redirect(RedirectToReviewPage(input.FestivalFilmId));
		}

		if (string.IsNullOrWhiteSpace(comment))
		{
			TempData["ReviewError"] = _localizer["Review_CommentRequired"].Value;
			return Redirect(RedirectToReviewPage(input.FestivalFilmId));
		}

		if (comment.Length > 1200)
		{
			TempData["ReviewError"] = _localizer["Review_CommentMaxLength"].Value;
			return Redirect(RedirectToReviewPage(input.FestivalFilmId));
		}

		// The form doesn't post UserId/ExternalFilmId/FilmId; we derive them server-side to satisfy FK constraints
		// and prevent tampering.
		var festivalFilm = await _festivalFilmRepository.GetByIdAsync(input.FestivalFilmId, cancellationToken);
		var externalFilmId = festivalFilm?.Film?.ExternalId ?? 0;
		var filmId = festivalFilm?.FilmId;

		// Business rule: Organizer/Admin reviews auto-approve; everyone else starts Pending.
		var autoApprove = User.IsInAnyRole(RoleConstants.AutoApproveRoles);
		var status = autoApprove ? "Approved" : "Pending";

		var normalized = new ReviewDto(
			Id: Guid.Empty,
			UserId: userId,
			FestivalFilmId: input.FestivalFilmId,
			ExternalFilmId: externalFilmId,
			Rating: input.Rating,
			Comment: comment,
			Status: status,
			DateUtc: DateTime.UtcNow,
			IsReported: false,
			HasBeenReported: false,
			FilmId: filmId);

		TempData.Remove("ReviewError");
		await _reviewService.AddAsync(normalized, cancellationToken);
		return Redirect(RedirectToReviewPage(input.FestivalFilmId));
	}

	/// <summary>Lazy-load JSON for replies (optional client fetch); same scoping rules as the public review page.</summary>
	[AllowAnonymous]
	[HttpGet("/Review/Replies")]
	public async Task<IActionResult> Replies(Guid festivalFilmId, Guid reviewId, CancellationToken cancellationToken)
	{
		if (festivalFilmId == Guid.Empty || reviewId == Guid.Empty)
		{
			return Json(new { items = Array.Empty<object>() });
		}

		var viewerUserId = Guid.TryParse(User.FindFirst("sub")?.Value, out var vid) ? vid : (Guid?)null;
		var isModerator = User.IsInAnyRole(RoleConstants.ModeratorRoles);
		var dtos = await _reviewReplyService.GetRepliesForReviewAsync(festivalFilmId, reviewId, viewerUserId, isModerator, cancellationToken);
		var distinctUserIds = dtos.Select(x => x.UserId).Distinct().ToList();
		var nameByUserId = new Dictionary<Guid, string>();
		foreach (var id in distinctUserIds)
		{
			var user = await _userRepository.GetByIdAsync(id, cancellationToken);
			if (user is not null)
			{
				nameByUserId[user.Id] = user.Name;
			}
		}

		var items = dtos.Select(d => new
		{
			d.Id,
			d.ReviewId,
			d.UserId,
			authorName = nameByUserId.TryGetValue(d.UserId, out var n) ? n : _localizer["Common_Unknown"].Value,
			comment = d.Comment,
			dateUtc = d.DateUtc,
			d.Status,
			d.IsReported,
			d.HasBeenReported
		}).ToList();

		return Json(new { items });
	}

	[Authorize]
	[HttpPost("/Review/Reply")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Reply(AddReviewReplyInputModel input, CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		if (input.FestivalFilmId == Guid.Empty || input.ReviewId == Guid.Empty)
		{
			TempData["ReviewError"] = _localizer["Review_InvalidRequest"].Value;
			return Redirect($"/Review?festivalFilmId={input.FestivalFilmId}#community-reviews");
		}

		var autoApprove = User.IsInAnyRole(RoleConstants.AutoApproveRoles);
		var initialStatus = autoApprove ? ReviewStatus.Approved : ReviewStatus.Pending;
		(Guid? _, UserMessage? error) = await _reviewReplyService.AddReplyAsync(userId, input.FestivalFilmId, input.ReviewId, input.Comment, initialStatus, cancellationToken);
		if (error is not null)
		{
			TempData["ReviewError"] = _localizer.LocalizeUserFacing(error);
		}
		else
		{
			TempData.Remove("ReviewError");
		}

		return Redirect($"/Review?festivalFilmId={input.FestivalFilmId}#review-card-{input.ReviewId}");
	}

	[Authorize(Roles = RoleConstants.CustomerRolesCsv)]
	[HttpPost("/Review/Report")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Report(Guid reviewId, Guid festivalFilmId, CancellationToken cancellationToken)
	{
		// Reporting is customer-only; staff moderation happens via Approve/Hide.
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		// Customers cannot report their own reviews.
		var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
			if (review is null || review.UserId == userId)
		{
			return RedirectToAction(nameof(Index), new { festivalFilmId });
		}

			// "Report-lock": if staff approved something after a report, we block re-reporting to avoid endless loops.
			if (review.HasBeenReported && review.Status == ReviewStatus.Approved)
			{
				return RedirectToAction(nameof(Index), new { festivalFilmId });
			}

		await _reviewService.ReportAsync(reviewId, cancellationToken);
		return RedirectToAction(nameof(Index), new { festivalFilmId });
	}

	[Authorize(Roles = RoleConstants.CustomerRolesCsv)]
	[HttpPost("/Review/ReportReply")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ReportReply(Guid replyId, Guid reviewId, Guid festivalFilmId, CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		var reply = await _reviewReplyService.GetReplyByIdAsync(replyId, cancellationToken);
		if (reply is null || reply.UserId == userId)
		{
			return RedirectToAction(nameof(Index), new { festivalFilmId });
		}

		var author = await _userRepository.GetByIdAsync(reply.UserId, cancellationToken);
		if (author is not null && (author.Roles.Contains(UserRole.Organizer) || author.Roles.Contains(UserRole.Admin)))
		{
			return RedirectToAction(nameof(Index), new { festivalFilmId });
		}

		if (reply.HasBeenReported && string.Equals(reply.Status, ReviewStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
		{
			return RedirectToAction(nameof(Index), new { festivalFilmId });
		}

		await _reviewReplyService.ReportReplyAsync(replyId, cancellationToken);
		return Redirect($"/Review?festivalFilmId={festivalFilmId}#review-card-{reviewId}");
	}

	[Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
	[HttpPost("/Review/ApproveReply")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ApproveReply(Guid replyId, Guid festivalFilmId, Guid reviewId, string? returnUrl, CancellationToken cancellationToken)
	{
		await _reviewReplyService.ApproveReplyAsync(replyId, cancellationToken);
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}

		return Redirect($"/Review?festivalFilmId={festivalFilmId}#review-card-{reviewId}");
	}

	[Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
	[HttpPost("/Review/HideReply")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> HideReply(Guid replyId, Guid festivalFilmId, Guid reviewId, string? returnUrl, CancellationToken cancellationToken)
	{
		await _reviewReplyService.HideReplyAsync(replyId, cancellationToken);
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}

		return Redirect($"/Review?festivalFilmId={festivalFilmId}#review-card-{reviewId}");
	}

	[Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
	[HttpPost("/Review/Hide")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Hide(Guid reviewId, Guid festivalFilmId, string? returnUrl, CancellationToken cancellationToken)
	{
		await _reviewService.HideReportedAsync(reviewId, cancellationToken);
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}

		if (festivalFilmId == Guid.Empty)
		{
			return RedirectToRoute("ReviewManage");
		}

		return RedirectToAction(nameof(Index), new { festivalFilmId });
	}

	[Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
	[HttpPost("/Review/Approve")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Approve(Guid reviewId, Guid festivalFilmId, string? returnUrl, CancellationToken cancellationToken)
	{
		await _reviewService.ApproveAsync(reviewId, cancellationToken);
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}

		return RedirectToAction(nameof(Index), new { festivalFilmId });
	}

	[Authorize(Roles = RoleConstants.OrganizerOrAdminRolesCsv)]
	[HttpGet("/review/manage", Name = "ReviewManage")] 
	public async Task<IActionResult> Manage(
		int page = 1,
		string? movieQuery = null,
		string? authorQuery = null,
		string? status = null,
		DateTime? dayUtc = null,
		CancellationToken cancellationToken = default)
	{
		// Management view supports paging + filters; page size aligns with card grid (3x3).
		var pageSize = 9;
		var result = await _reviewService.GetAllForManagementPageAsync(
			page,
			pageSize,
			movieQuery,
			authorQuery,
			status,
			dayUtc,
			cancellationToken);
		var viewModels = result.Items.Select(x => x.ToViewModel()).ToList();

		var replyDtos = await _reviewReplyService.GetRepliesByReviewIdsForManagementAsync(
			viewModels.Select(x => x.Id).ToList(),
			cancellationToken);
		var repliesByReviewId = replyDtos.GroupBy(x => x.ReviewId).ToDictionary(g => g.Key, g => g.ToList());

		var distinctUserIds = viewModels
			.Select(x => x.UserId)
			.Concat(replyDtos.Select(x => x.UserId))
			.Distinct()
			.ToList();
		var staffByUserId = new Dictionary<Guid, bool>();
		var nameByUserId = new Dictionary<Guid, string>();
		foreach (var id in distinctUserIds)
		{
			var user = await _userRepository.GetByIdAsync(id, cancellationToken);
			if (user is null)
			{
				continue;
			}

			staffByUserId[user.Id] = user.Roles.Contains(UserRole.Organizer) || user.Roles.Contains(UserRole.Admin);
			nameByUserId[user.Id] = user.Name;
		}

		foreach (var vm in viewModels)
		{
			if (!repliesByReviewId.TryGetValue(vm.Id, out var list))
			{
				continue;
			}

			vm.Replies = list.Select(r => r.ToViewModel()).ToList();
			foreach (var replyVm in vm.Replies)
			{
				replyVm.AuthorName = nameByUserId.TryGetValue(replyVm.UserId, out var n) ? n : _localizer["Common_Unknown"].Value;
				replyVm.IsStaffAuthor = staffByUserId.TryGetValue(replyVm.UserId, out var st) && st;
			}
		}

		return View("~/Views/Review/Manage.cshtml", new ReviewManagePageViewModel
		{
			Page = result.Page,
			HasNext = result.HasNext,
			Reviews = viewModels,
			MovieQuery = movieQuery,
			AuthorQuery = authorQuery,
			Status = status,
			DayUtc = dayUtc
		});
	}

	// private bool TryGetCurrentUserId(out Guid userId)
	// {
	// 	return Guid.TryParse(User.FindFirst("sub")?.Value, out userId);
	// }
}
