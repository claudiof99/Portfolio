using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Web.Services;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Recommendations;
using UmaFestHub.Application.Pricing;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Security;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Web.Controllers;
[Route("festivals")]
public class FestivalController : Controller
{
	private readonly IFestivalService _festivalService;
	private readonly ITmdbClient _tmdbClient;
	private readonly IFilmService _filmService;
	private readonly IFestivalFilmService _festivalFilmService;
	private readonly IProductService _productService;
	private readonly IEntitlementService _entitlementService;
	private readonly IPurchaseService _purchaseService;
	private readonly IProductRepository _productRepository;
	private readonly IPersonalListService _personalListService;
	private readonly IRecommendationService _recommendationService;
	private readonly IPricingService _pricingService;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public FestivalController(
		IFestivalService festivalService,
		ITmdbClient tmdbClient,
		IFilmService filmService,
		IFestivalFilmService festivalFilmService,
		IPersonalListService personalListService,
		IProductService productService,
		IEntitlementService entitlementService,
		IPurchaseService purchaseService,
		IProductRepository productRepository,
		IRecommendationService recommendationService,
		IPricingService pricingService,
		IStringLocalizer<SharedResources> localizer)
	{
		_festivalService = festivalService;
		_tmdbClient = tmdbClient;
		_filmService = filmService;
		_festivalFilmService = festivalFilmService;
		_productService = productService;
		_entitlementService = entitlementService;
		_purchaseService = purchaseService;
		_productRepository = productRepository;
		_personalListService = personalListService;
		_recommendationService = recommendationService;
		_pricingService = pricingService;
		_localizer = localizer;
	}
	[Authorize(Roles = "Organizer,Admin")]
	// [HttpPost("{festivalId:guid}/import-film", Name = "FestivalImportFilm")]
	[HttpPost("import/add-film", Name = "FestivalImportFilm")]

	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ImportFilmToFestival(Guid festivalId, int tmdbId, CancellationToken cancellationToken)
	{
		if (tmdbId <= 0 || festivalId == Guid.Empty)
		{
			return BadRequest(_localizer["Festival_ImportIdsRequired"].Value);
		}

		try
		{
			var festivalFilmId = await _festivalFilmService.ImportFromTmdbAsync(festivalId, tmdbId, cancellationToken);
			return Json(new { festivalFilmId, message = _localizer["Festival_ImportSuccess"].Value });
		}
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status502BadGateway, _localizer["Festival_ImportFailed"].Value);
		}
	}

	[Authorize(Roles = "Admin")]
	[HttpPost("remove-film", Name = "FestivalRemoveFilm")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RemoveFilm(Guid festivalFilmId, Guid festivalId, CancellationToken cancellationToken)
	{
		try
		{
			// Execute the un-linking of the film from the festival lineup
			await _festivalFilmService.DeleteAsync(festivalFilmId, cancellationToken);
			return RedirectToAction("Details", new { id = festivalId });
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException)
		{
			TempData["ErrorMessage"] = _localizer["Festival_CannotRemoveFilm"].Value;
			return RedirectToAction("Details", new { id = festivalId });
		}
	}

	[HttpGet("", Name = "FestivalIndex")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		IReadOnlyList<FestivalDto> festivals;
		if (User.IsInAnyRole(RoleConstants.ModeratorRoles))
		{
			festivals = await _festivalService.GetAllAsync(cancellationToken);
		}
		else
		{
			// Public browse — only show non-hidden festivals.
			festivals = await _festivalService.GetAllVisibleAsync(cancellationToken);
		}

		var viewModels = festivals.Select(x => x.ToViewModel()).ToList();
		var coverImages = await _festivalFilmService.GetCoverImageUrlsByFestivalIdsAsync(
			viewModels.Select(f => f.Id).ToList(),
			cancellationToken);

		foreach (var festival in viewModels)
		{
			if (coverImages.TryGetValue(festival.Id, out var coverImageUrl))
			{
				festival.CoverImageUrl = coverImageUrl;
			}
		}

		return View(viewModels);
	}

	[HttpGet("/festivals/{id:guid}")]
	[HttpGet("/festival/details/{id:guid}", Name = "FestivalDetails")]
	public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
	{
		if (id == Guid.Empty)
		{
			return View();
		}

		var festival = await _festivalService.GetByIdAsync(id, cancellationToken);
		if (festival is null) return NotFound();

		var vm = festival.ToViewModel();
		ViewBag.IsFestivalStarted = DateTime.UtcNow >= festival.StartDateUtc;
		ViewBag.IsFestivalEnded = DateTime.UtcNow > festival.EndDateUtc;
		ViewBag.FestivalStartDate = festival.StartDateUtc;
		var dailyPass = await _productService.GetDailyPassDtoAsync(id, cancellationToken);
		var completePass = await _productService.GetCompletePassDtoAsync(id, cancellationToken);
		vm.DailyPassPrice = dailyPass?.Price;
		vm.CompletePassPrice = completePass?.Price;

		// Calculate early bird discounted prices for passes
		var userId = Guid.Empty;
		if (User.Identity?.IsAuthenticated == true)
		{
			var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
			Guid.TryParse(userIdStr, out userId);
		}

		if (festival.EarlyBirdDiscountPercent.HasValue && festival.EarlyBirdDaysBeforeStart.HasValue && userId != Guid.Empty)
		{
			var purchaseDate = DateTime.UtcNow;
			if (dailyPass != null)
			{
				var dailyPassProduct = await _productRepository.GetByIdAsync(dailyPass.Id, cancellationToken);
				if (dailyPassProduct != null)
				{
					vm.DailyPassDiscountedPrice = await _pricingService.GetPriceAsync(dailyPassProduct, userId, purchaseDate, cancellationToken);
				}
			}
			if (completePass != null)
			{
				var completePassProduct = await _productRepository.GetByIdAsync(completePass.Id, cancellationToken);
				if (completePassProduct != null)
				{
					vm.CompletePassDiscountedPrice = await _pricingService.GetPriceAsync(completePassProduct, userId, purchaseDate, cancellationToken);
				}
			}
		}

		ViewBag.DailyPassId = dailyPass?.Id;
		ViewBag.CompletePassId = completePass?.Id;

		var purchasedProductIds = new System.Collections.Generic.HashSet<Guid>();
		var purchasedPurchaseDates = new System.Collections.Generic.Dictionary<Guid, DateTime>();
		if (userId != Guid.Empty)
		{
			var history = await _purchaseService.GetHistoryAsync(userId, cancellationToken);
			foreach (var p in history.Where(x => string.Equals(x.Status, "Completed", StringComparison.OrdinalIgnoreCase)))
			{
				foreach (var item in p.Items)
				{
					purchasedProductIds.Add(item.ProductId);
					if (!purchasedPurchaseDates.ContainsKey(item.ProductId) || purchasedPurchaseDates[item.ProductId] < p.DateUtc)
					{
						purchasedPurchaseDates[item.ProductId] = p.DateUtc;
					}
				}
			}
		}
		ViewBag.PurchasedProductIds = purchasedProductIds;

		var hasDailyPass = dailyPass != null && purchasedProductIds.Contains(dailyPass.Id);
		var hasCompletePass = completePass != null && purchasedProductIds.Contains(completePass.Id);

		var festivalFilms = await _festivalFilmService.GetByFestivalIdAsync(id, cancellationToken);
		var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
		var filmIds = festivalFilms.Select(ff => ff.FilmId).Distinct().ToList();
		var catalogFilms = await _filmService.GetByIdsAsync(filmIds, cancellationToken);
		var localizedFilms = await _filmService.LocalizeFilmsAsync(catalogFilms, tmdbLanguage, cancellationToken);
		var localizedByFilmId = localizedFilms.ToDictionary(f => f.Id);

		var accessMap = new System.Collections.Generic.Dictionary<Guid, bool>();
		var watchLinkMap = new System.Collections.Generic.Dictionary<Guid, string>();
		foreach (var ff in festivalFilms)
		{
			var hasAccess = false;
			string? watchLink = null;
			if (userId != Guid.Empty)
			{
				SessionDto? targetSession = null;

				if (hasDailyPass || hasCompletePass)
				{
					hasAccess = true;
					// Pass holders use a FixedSession/Premier, not AccessWindow
					targetSession = ff.Sessions?
						.FirstOrDefault(s => !s.SessionType.ToString()
							.Contains("AccessWindow", StringComparison.OrdinalIgnoreCase))
						?? ff.Sessions?.FirstOrDefault();
				}

				if (!hasAccess)
				{
					var rentalDto = await _productService.GetRentalDtoAsync(ff.Id, cancellationToken);
					if (rentalDto != null && purchasedProductIds.Contains(rentalDto.Id))
					{
						if (purchasedPurchaseDates.TryGetValue(rentalDto.Id, out var purchaseDate)
							&& purchaseDate.AddHours(rentalDto.DurationValue) > DateTime.UtcNow)
						{
							hasAccess = true;
							// Rental holders use the AccessWindow session
							targetSession = ff.Sessions?
								.FirstOrDefault(s => s.SessionType.ToString()
									.Contains("AccessWindow", StringComparison.OrdinalIgnoreCase))
								?? ff.Sessions?.FirstOrDefault();
						}
					}
				}

				if (!hasAccess && ff.Sessions != null)
				{
					// Last resort: check ticket per session (no EntitlementService call to avoid double render)
					foreach (var session in ff.Sessions)
					{
						if (await _entitlementService.CanWatchMovieAsync(userId, id, ff.Id, session.Id, cancellationToken))
						{
							hasAccess = true;
							targetSession = session;
							break;
						}
					}
				}

				if (hasAccess && targetSession != null)
					watchLink = $"/festivals/{id}/sessions/{targetSession.Id}/watch";

			}

			accessMap[ff.Id] = hasAccess;
			if (hasAccess && watchLink != null)
			{
				watchLinkMap[ff.Id] = watchLink;
			}
			
			// Track active rentals separately for the view
			var hasActiveRental = false;
			var rental = await _productService.GetRentalDtoAsync(ff.Id, cancellationToken);
			if (rental != null && purchasedProductIds.Contains(rental.Id))
			{
				if (purchasedPurchaseDates.TryGetValue(rental.Id, out var pDate)
					&& pDate.AddHours(rental.DurationValue) > DateTime.UtcNow)
				{
					hasActiveRental = true;
				}
			}
			if (ViewBag.RentalAccessMap == null) ViewBag.RentalAccessMap = new System.Collections.Generic.Dictionary<Guid, bool>();
			ViewBag.RentalAccessMap[ff.Id] = hasActiveRental;
		}
		ViewBag.AccessMap = accessMap;
		ViewBag.WatchLinkMap = watchLinkMap;

		var favoriteFilmIds = new HashSet<Guid>();
		var watchlistFilmIds = new HashSet<Guid>();
		if (userId != Guid.Empty)
		{
			// Toggle labels on lineup cards (POST targets PersonalListController Add/Remove).
			var ids = await _personalListService.GetListAsync(userId, PersonalListType.Favorites, cancellationToken);
			favoriteFilmIds = ids.ToHashSet();
			var watchlistIds = await _personalListService.GetListAsync(userId, PersonalListType.Watchlist, cancellationToken);
			watchlistFilmIds = watchlistIds.ToHashSet();
		}

		vm.Films = festivalFilms.Select(ff =>
		{
			localizedByFilmId.TryGetValue(ff.FilmId, out var localizedFilm);
			return new FestivalFilmViewModel
		{
			Id = ff.Id,
			FilmId = ff.FilmId,
			IsFavorite = favoriteFilmIds.Contains(ff.FilmId),
			IsWatchlist = watchlistFilmIds.Contains(ff.FilmId),
			FilmName = string.IsNullOrWhiteSpace(localizedFilm?.Name ?? ff.FilmName) ? _localizer["Common_Unknown"].Value : (localizedFilm?.Name ?? ff.FilmName),
			ImageUrl = ff.ImageUrl,
			FilmDescription = localizedFilm?.Description ?? ff.FilmDescription,
			DurationMinutes = ff.DurationMinutes,
			Genres = localizedFilm?.Genres ?? ff.Genres ?? new List<string>(),
			SessionCount = ff.SessionCount,
			IsWorldPremier = ff.IsWorldPremier,
			FilmUrl = ff.FilmUrl,
			Sessions     = ff.Sessions.Select(s => new SessionViewModel
			{
				Id           = s.Id,
				SessionType  = s.SessionType.ToString(),
				StartTimeUtc = s.StartTimeUtc,
				EndTimeUtc   = s.EndTimeUtc,
				TicketId     = _productRepository.GetTicketAsync(s.Id, cancellationToken).Result?.Id
			}).ToList()
		};
		}).ToList();

		// Recommendations — only for authenticated users with a real userId
		if (userId != Guid.Empty)
		{
			var recs = await _recommendationService.GetAsync(userId, id, 6, cancellationToken);
			vm.Recommendations = recs.Select(r => r.ToViewModel()).ToList();
		}
		else
		{
			vm.Recommendations = Array.Empty<RecommendationViewModel>();
		}

		return View(vm);
	}

	[Authorize(Roles = "Organizer,Admin")]
	[HttpGet("create", Name = "FestivalCreateView")]
	public IActionResult Create() => View(new FestivalViewModel());

	[Authorize(Roles = "Organizer,Admin")]
	[HttpPost("create", Name = "FestivalCreate")]
	public async Task<IActionResult> Create(FestivalViewModel model, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		try
		{
			var dto = new FestivalDto(model.Id, model.Name, model.Description, model.StartDateUtc, model.EndDateUtc,
				model.EarlyBirdDiscountPercent, model.EarlyBirdDaysBeforeStart);
			var id = await _festivalService.CreateAsync(dto, cancellationToken);
			return RedirectToRoute("FestivalDetails", new { id });
		}
		catch (ArgumentException ex)
		{
			ModelState.AddModelError(string.Empty, _localizer.LocalizeKeyOrFallback(ex.Message));
			return View(model);
		}
	}

	[Authorize(Roles = "Organizer,Admin")]
	[HttpGet("/festival/{id:guid}/edit", Name = "FestivalEdit")]
	public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
	{
		var festival = await _festivalService.GetByIdAsync(id, cancellationToken);
		if (festival is null) return NotFound();
		return View(festival.ToViewModel());
	}

	[Authorize(Roles = "Organizer,Admin")]
	[HttpPost("/festival/{id:guid}/edit", Name = "FestivalEditPost")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(Guid id, FestivalViewModel model, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return View(model);

		try
		{
			var dto = new FestivalDto(id, model.Name, model.Description, model.StartDateUtc, model.EndDateUtc,
				model.EarlyBirdDiscountPercent, model.EarlyBirdDaysBeforeStart);
			await _festivalService.UpdateAsync(dto, cancellationToken);
			return RedirectToRoute("FestivalDetails", new { id });
		}
		catch (ArgumentException ex)
		{
			ModelState.AddModelError(string.Empty, _localizer.LocalizeKeyOrFallback(ex.Message));
			return View(model);
		}
	}

	[Authorize(Roles = "Admin")]
	[HttpPost("/festival/{id:guid}/delete", Name = "FestivalDelete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			await _festivalService.DeleteAsync(id, cancellationToken);
			TempData["SuccessMessage"] = _localizer["Festival_DeleteSuccess"].Value;
			return RedirectToAction(nameof(Index));
		}
		catch (InvalidOperationException ex) when (ex.Message == "Festival_HasPurchases_UseHide")
		{
			// Festival has purchases — hard delete is blocked. Instruct admin to use Hide.
			TempData["ErrorMessage"] = _localizer["Festival_HasPurchases_UseHide"].Value;
			return RedirectToAction(nameof(Details), new { id });
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException)
		{
			TempData["ErrorMessage"] = _localizer["Festival_CannotDeletePurchases"].Value;
			TempData["InfoMessage"] = _localizer["Festival_DeleteInfoPurchases"].Value;
			return RedirectToAction(nameof(Details), new { id });
		}
	}

	[Authorize(Roles = "Admin")]
	[HttpPost("/festival/{id:guid}/hide", Name = "FestivalHide")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Hide(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			await _festivalService.SetHiddenAsync(id, true, cancellationToken);
			return Ok(new { isHidden = true, message = _localizer["Festival_HideSuccess"].Value });
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

	[Authorize(Roles = "Admin")]
	[HttpPost("/festival/{id:guid}/unhide", Name = "FestivalUnhide")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Unhide(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			await _festivalService.SetHiddenAsync(id, false, cancellationToken);
			return Ok(new { isHidden = false, message = _localizer["Festival_UnhideSuccess"].Value });
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

	[Authorize(Roles = "Organizer,Admin")]
	[HttpPost("import/search", Name = "FestivalTmdbSearch")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TmdbSearch(string query, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			return BadRequest(_localizer["Festival_TmdbQueryRequired"].Value);
		}

		try
		{
			var language = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
			var results = await _tmdbClient.SearchMoviesAsync(query, language, cancellationToken);
			
			// Format the results for the frontend to easily display a thumbnail, title, and description
			var formattedResults = results.Select(r => new
			{
				id = r.Id,
				title = r.Title,
				description = string.IsNullOrWhiteSpace(r.Overview) ? _localizer["Manage_ImportNoOverview"].Value : r.Overview,
				imageUrl = !string.IsNullOrWhiteSpace(r.PosterPath) ? $"https://image.tmdb.org/t/p/w200{r.PosterPath}" : null,
				releaseDate = r.ReleaseDate
			});

			return Json(formattedResults);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(_localizer.LocalizeKeyOrFallback(ex.Message, "Festival_TmdbApiKeyNotConfigured"));
		}
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status502BadGateway, _localizer["Festival_TmdbSearchFailed"].Value);
		}
	}

	[Authorize(Roles = "Organizer,Admin")]
	[HttpPost("import/details", Name = "FestivalTmdbDetails")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TmdbDetails(int tmdbId, CancellationToken cancellationToken)
	{
		if (tmdbId <= 0)
		{
			return BadRequest(_localizer["Festival_TmdbIdRequired"].Value);
		}

		try
		{
			var language = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
			var details = await _tmdbClient.GetMovieDetailsAsync(tmdbId, language, cancellationToken);
			return Json(details);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(_localizer.LocalizeKeyOrFallback(ex.Message, "Festival_TmdbApiKeyNotConfigured"));
		}
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status502BadGateway, _localizer["Festival_TmdbDetailsFailed"].Value);
		}
	}
}
