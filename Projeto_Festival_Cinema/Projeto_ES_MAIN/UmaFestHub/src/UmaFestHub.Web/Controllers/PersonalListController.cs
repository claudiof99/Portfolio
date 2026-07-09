using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.Services;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

/// <summary>
/// Authenticated UX for user film lists: GET <c>/PersonalList</c> (tabs, filters, cards), POST add/remove entries.
/// List membership uses <see cref="IPersonalListService"/>; full film rows via <see cref="IFilmService"/>; festival filtering uses purchase + lineup data.
/// The Seen tab uses <see cref="PersonalListType.Watched"/>; rows may be created automatically when users open entitled festival watch URLs (Observer pipeline).
/// Same Seen data is also shown without tabs at <c>/watchHistory</c> via <see cref="WatchHistoryController"/>.
/// </summary>
[Authorize]
[Route("PersonalList")]
public class PersonalListController : Controller
{
	private readonly IPersonalListService _personalListService;
	private readonly IFilmService _filmService;
	private readonly IPurchaseRepository _purchaseRepository;
	private readonly IFestivalFilmRepository _festivalFilmRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public PersonalListController(
		IPersonalListService personalListService,
		IFilmService filmService,
		IPurchaseRepository purchaseRepository,
		IFestivalFilmRepository festivalFilmRepository,
		IStringLocalizer<SharedResources> localizer)
	{
		_personalListService = personalListService;
		_filmService = filmService;
		_purchaseRepository = purchaseRepository;
		_festivalFilmRepository = festivalFilmRepository;
		_localizer = localizer;
	}

	/// <summary>
	/// Shows one list type with optional title/genre filters and festival scoping driven by purchases vs lineup overlap.
	/// </summary>
	/// <param name="type">Watchlist, Favorites, or Seen (<see cref="PersonalListType.Watched"/>).</param>
	/// <param name="title">Optional title search (also matches genre text in list cards).</param>
	/// <param name="genre">Optional genre filter.</param>
	/// <param name="festivalId">Optional festival scope when the user’s purchases overlap that festival’s lineup.</param>
	[HttpGet("/PersonalList")]
	public async Task<IActionResult> Index(
		PersonalListType type = PersonalListType.Watchlist,
		string? title = null,
		string? genre = null,
		Guid? festivalId = null,
		CancellationToken cancellationToken = default)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		// Shared with /watchHistory for Seen (Watched) — see PersonalListPageModelBuilder.
		var vm = await PersonalListPageModelBuilder.BuildAsync(
			userId,
			type,
			title,
			genre,
			festivalId,
			_personalListService,
			_filmService,
			_purchaseRepository,
			_festivalFilmRepository,
			_localizer,
			cancellationToken);

		ViewData["Title"] = vm.PageTitle;
		return View(vm);
	}

	/// <summary>Adds a film id to the given list type; redirects to <paramref name="returnUrl"/> when local.</summary>
	[HttpPost("Add")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Add(PersonalListType type, Guid filmId, string? returnUrl = null, CancellationToken cancellationToken = default)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		await _personalListService.AddFilmAsync(userId, type, filmId, cancellationToken);
		return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
			? Redirect(returnUrl)
			: RedirectToAction(nameof(Index), new { type });
	}

	/// <summary>
	/// Removes a film from a list (including Seen / <see cref="PersonalListType.Watched"/>). Used from
	/// <c>/PersonalList</c>, <c>/watchHistory</c>, and home carousels via <paramref name="returnUrl"/>.
	/// </summary>
	[HttpPost("Remove")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Remove(PersonalListType type, Guid filmId, string? returnUrl = null, CancellationToken cancellationToken = default)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		await _personalListService.RemoveFilmAsync(userId, type, filmId, cancellationToken);
		return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
			? Redirect(returnUrl)
			: RedirectToAction(nameof(Index), new { type });
	}
}
