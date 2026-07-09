using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.Services;

namespace UmaFestHub.Web.Controllers;

/// <summary>
/// Seen list at <c>/watchHistory</c>: same backing data and filters as <c>/PersonalList?type=Watched</c>, without the
/// watchlist / favorites tab bar. List rows are still <see cref="PersonalListType.Watched"/> in <c>PersonalLists</c>
/// (written when users open entitled watch URLs — see <c>FilmsWatchedListObserver</c>).
/// </summary>
[Authorize]
public class WatchHistoryController : Controller
{
	private readonly IPersonalListService _personalListService;
	private readonly IFilmService _filmService;
	private readonly IPurchaseRepository _purchaseRepository;
	private readonly IFestivalFilmRepository _festivalFilmRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public WatchHistoryController(
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
	/// Renders the Seen (Watched) list. Primary route for link generation is <c>/watchHistory</c>; kebab-case kept for bookmarks.
	/// </summary>
	/// <param name="title">Optional title filter (same as personal list).</param>
	/// <param name="genre">Optional genre filter.</param>
	/// <param name="festivalId">Optional festival filter.</param>
	[HttpGet("/watchHistory", Name = "WatchHistoryIndex")]
	[HttpGet("/watch-history")] // legacy / alternate URL
	public async Task<IActionResult> Index(
		string? title = null,
		string? genre = null,
		Guid? festivalId = null,
		CancellationToken cancellationToken = default)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		// Always Seen: same builder as PersonalList when type is Watched.
		var vm = await PersonalListPageModelBuilder.BuildAsync(
			userId,
			PersonalListType.Watched,
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
}
