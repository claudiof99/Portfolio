using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Web.Security;
using UmaFestHub.Application.Observers.FilmsWatched;

namespace UmaFestHub.Web.Controllers;

/// <summary>Festival session CRUD and entitled watch pages (<see cref="Watch"/>, <see cref="WatchFilm"/>), including Seen-list notification via <see cref="IFilmWatchedNotifier"/>.</summary>
[Route("sessions")]
public class SessionController : Controller
{
	private readonly ISessionService _sessionService;
	private readonly ISessionAccessService _sessionAccessService;
	private readonly IFestivalFilmService _festivalFilmService;
	private readonly IProductRepository _productRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly IFilmService _filmService;
	private readonly IEntitlementService _entitlementService;
	private readonly IPurchaseService _purchaseService;
	private readonly IProductService _productService;
	private readonly IFestivalService _festivalService;
	/// <summary>Publishes “film watched” after session/pass access succeeds so Seen list observers can run.</summary>
	private readonly IFilmWatchedNotifier _filmWatchedNotifier;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public SessionController(
		ISessionService sessionService, 
		IFestivalFilmService festivalFilmService,
		ISessionAccessService sessionAccessService,
		IProductRepository productRepository,
		ISessionRepository sessionRepository,
		IFilmService filmService,
		IEntitlementService entitlementService,
		IPurchaseService purchaseService,
		IProductService productService,
		IFestivalService festivalService,
		IFilmWatchedNotifier filmWatchedNotifier,
		IStringLocalizer<SharedResources> localizer)
	{
		_sessionService = sessionService;
		_festivalFilmService = festivalFilmService;
		_sessionAccessService = sessionAccessService;
		_productRepository = productRepository;
		_sessionRepository = sessionRepository;
		_filmService = filmService;
		_entitlementService = entitlementService;
		_purchaseService = purchaseService;
		_productService = productService;
		_festivalService = festivalService;
		_filmWatchedNotifier = filmWatchedNotifier;
		_localizer = localizer;
	}
	
	[HttpGet("", Name = "SessionIndex")]
	public async Task<IActionResult> Index(Guid festivalId, Guid festivalFilmId, CancellationToken cancellationToken)
	{
		var festivalFilm = await _festivalFilmService.GetByIdAsync(festivalFilmId, cancellationToken);
        if (festivalFilm is null || festivalFilm.FestivalId != festivalId)
            return NotFound();

		ViewBag.FestivalId = festivalId;
    	ViewBag.FestivalFilmId = festivalFilmId;

		var sessions = await _sessionService.GetByFestivalFilmIdAsync(festivalFilmId, cancellationToken);
        return View(sessions.Select(x => x.ToViewModel()).ToList());

	}

	
	[HttpGet("create", Name = "SessionCreateView")]
	public IActionResult Create(Guid festivalId, Guid festivalFilmId)
	{
		ViewBag.FestivalId = festivalId;
		ViewBag.FestivalFilmId = festivalFilmId;
		return View(new SessionViewModel { FestivalFilmId = festivalFilmId, Price = 10.00m });
	}


	[HttpPost("create", Name = "SessionCreate")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(SessionViewModel model, CancellationToken cancellationToken)
	{
		var priceStr = Request.Form["Price"].ToString().Replace(",", ".");
		ModelState.Clear();

		if (model.SessionType?.ToString().Contains("AccessWindow") == true)
		{
			model.Price = 0m;
		}
		else if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedPrice) && parsedPrice >= 0)
		{
			model.Price = parsedPrice;
		}
		else
		{
			TempData["ErrorMessage"] = _localizer["Common_InvalidPrice"].Value;
			var film = await _festivalFilmService.GetByIdAsync(model.FestivalFilmId, cancellationToken);
			if (film != null)
			{
				ViewBag.FestivalId = film.FestivalId;
				ViewBag.FestivalFilmId = film.Id;
			}
			return View(model);
		}

		var dto = new SessionDto(model.Id, model.FestivalFilmId, model.SessionType, model.StartTimeUtc, model.EndTimeUtc);
		var (succeeded, sessionId, error) = await _sessionService.CreateAsync(dto, cancellationToken);
		if (!succeeded)
		{
			TempData["ErrorMessage"] = error is null
				? _localizer["Session_CreateFailed"].Value
				: _localizer.LocalizeUserFacing(error);
			var film = await _festivalFilmService.GetByIdAsync(model.FestivalFilmId, cancellationToken);
			if (film != null)
			{
				ViewBag.FestivalId = film.FestivalId;
				ViewBag.FestivalFilmId = film.Id;
			}
			return View(model);
		}

		if (sessionId.HasValue && model.SessionType != SessionType.AccessWindow)
		{
			var ticket = new Ticket
			{
				Id = Guid.NewGuid(),
				SessionId = sessionId.Value,
				Price = model.Price,
				TicketNumber = $"TKT-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}"
			};
			await _productRepository.AddAsync(ticket, cancellationToken);
		}

		var festivalFilm = await _festivalFilmService.GetByIdAsync(model.FestivalFilmId, cancellationToken);
		if (festivalFilm is null)
			return NotFound();

		return RedirectToRoute("SessionIndex", new 
		{ 
			festivalId = festivalFilm.FestivalId, 
			festivalFilmId = model.FestivalFilmId 
		});
	}

	/// <summary>Entitled session playback page. Fires <see cref="IFilmWatchedNotifier"/> so Seen list (and other observers) update.</summary>
	[Authorize]
	[HttpGet("/festivals/{festivalId:guid}/sessions/{sessionId:guid}/watch", Name = "SessionWatch")]
	public async Task<IActionResult> Watch(
		Guid festivalId,
		Guid sessionId,
		CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		// Fetch session first so we can pass its start time to the access check
		var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
		if (session == null) return NotFound();

		var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);
		if (festival == null) return NotFound();

		if (DateTime.UtcNow < festival.StartDateUtc)
		{
			TempData["AccessError"] = _localizer["Session_FestivalNotStarted", festival.Name, festival.StartDateUtc.ToString("yyyy-MM-dd")].Value;
			return RedirectToRoute("FestivalDetails", new { id = festivalId });
		}

		// Populate context with request data
		var context = new SessionAccessDto(
			UserId: userId,
			SessionId: sessionId,
			FestivalId: festivalId,
			FestivalFilmId: session.FestivalFilmId,
			NowUtc: DateTime.UtcNow,
			SessionStartUtc: session.StartTimeUtc,
			SessionEndUtc: session.EndTimeUtc,
			FestivalEndUtc: festival.EndDateUtc);

		var (allowed, accessError) = await _sessionAccessService.ValidateAccessAsync(context, cancellationToken);

		if (!allowed)
		{
			TempData["AccessError"] = accessError is null
				? _localizer["SessionAccess_Denied"].Value
				: _localizer.LocalizeUserFacing(accessError);
			return RedirectToRoute("FestivalDetails", new { id = festivalId });
		}

		var festivalFilm = await _festivalFilmService.GetByIdAsync(session.FestivalFilmId, cancellationToken);
		if (festivalFilm == null) return NotFound();

		var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
		var film = await _filmService.GetByIdLocalizedAsync(festivalFilm.FilmId, tmdbLanguage, cancellationToken);
		if (film == null) return NotFound();

		// Seen list (Observer): record catalog film for user after entitlement — does not replace access checks above.
		await _filmWatchedNotifier.NotifyFilmWatchedAsync(new FilmWatchedContext(userId, film.Id), cancellationToken);

		var viewModel = new WatchViewModel
		{
			FilmName = film.Name,
			FilmId = film.Id,
			TrailerEmbedUrl = "https://www.youtube.com/embed/zSWdZVtXT7E?autoplay=1&rel=0",
			SessionType = session.SessionType?.ToString() ?? string.Empty,
			SessionStartUtc = session.StartTimeUtc,
			FestivalId = festivalId,
			FestivalFilmId = session.FestivalFilmId,
			SessionId = sessionId,
			FilmDescription = film.Description,
			PosterUrl = film.ImageUrl,
			DurationMinutes = film.DurationMinutes,
			Genres = film.Genres,
			FestivalName = festival.Name
		};

		return View(viewModel);
	}

	/// <summary>Entitled pass/rental film playback page. Fires <see cref="IFilmWatchedNotifier"/> for Seen list and other observers.</summary>
	[Authorize]
	[HttpGet("/festivals/{festivalId:guid}/films/{festivalFilmId:guid}/watch", Name = "FilmWatch")]
	public async Task<IActionResult> WatchFilm(
		Guid festivalId,
		Guid festivalFilmId,
		CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);
		if (festival == null) return NotFound();

		if (DateTime.UtcNow < festival.StartDateUtc)
		{
			TempData["AccessError"] = _localizer["Session_FestivalNotStarted", festival.Name, festival.StartDateUtc.ToString("yyyy-MM-dd")].Value;
			return RedirectToRoute("FestivalDetails", new { id = festivalId });
		}

		var festivalFilm = await _festivalFilmService.GetByIdAsync(festivalFilmId, cancellationToken);
		if (festivalFilm == null) return NotFound();

		var context = new SessionAccessDto(
			UserId: userId,
			SessionId: null,
			FestivalId: festivalId,
			FestivalFilmId: festivalFilmId,
			NowUtc: DateTime.UtcNow,
			SessionStartUtc: DateTime.UtcNow,
			SessionEndUtc: DateTime.UtcNow.AddDays(1),
			FestivalEndUtc: festival.EndDateUtc);

		var (allowed, accessError) = await _sessionAccessService.ValidateAccessAsync(context, cancellationToken);
		if (!allowed)
		{
			TempData["AccessError"] = accessError is null
				? _localizer["SessionAccess_Denied"].Value
				: _localizer.LocalizeUserFacing(accessError);
			return RedirectToRoute("FestivalDetails", new { id = festivalId });
		}

		var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
		var film = await _filmService.GetByIdLocalizedAsync(festivalFilm.FilmId, tmdbLanguage, cancellationToken);
		if (film == null) return NotFound();

		// Seen list (Observer): same pipeline as session watch once access is granted.
		await _filmWatchedNotifier.NotifyFilmWatchedAsync(new FilmWatchedContext(userId, film.Id), cancellationToken);

		var viewModel = new WatchViewModel
		{
			FilmName = film.Name,
			FilmId = film.Id,
			TrailerEmbedUrl = "https://www.youtube.com/embed/zSWdZVtXT7E?autoplay=1&rel=0",
			SessionType = _localizer["Session_TypePassAccess"].Value,
			SessionStartUtc = DateTime.UtcNow,
			FestivalId = festivalId,
			FestivalFilmId = festivalFilmId,
			SessionId = null,
			FilmDescription = film.Description,
			PosterUrl = film.ImageUrl,
			DurationMinutes = film.DurationMinutes,
			Genres = film.Genres,
			FestivalName = festival.Name
		};

		return View("Watch", viewModel);
	}
}