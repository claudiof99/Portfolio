using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Extensions;

namespace UmaFestHub.Web.Controllers;

/// <summary>Live Watch Party — entitled users watch a synced YouTube stream with group chat.</summary>
[Authorize]
[Route("watchparty")]
public class WatchPartyController : Controller
{
	private readonly IEntitlementService _entitlementService;
	private readonly IFestivalFilmService _festivalFilmService;
	private readonly IFilmService _filmService;
	private readonly IFestivalService _festivalService;

	public WatchPartyController(
		IEntitlementService entitlementService,
		IFestivalFilmService festivalFilmService,
		IFilmService filmService,
		IFestivalService festivalService)
	{
		_entitlementService = entitlementService;
		_festivalFilmService = festivalFilmService;
		_filmService = filmService;
		_festivalService = festivalService;
	}

	[HttpGet("", Name = "WatchPartyIndex")]
	public async Task<IActionResult> Index(
		Guid festivalId, Guid festivalFilmId, Guid? sessionId,
		CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		var hasAccess = await _entitlementService.CanWatchMovieAsync(
			userId, festivalId, festivalFilmId, sessionId, cancellationToken);

		if (!hasAccess)
		{
			// Follow existing "?toast=access-denied" pattern from Program.cs OnRedirectToAccessDenied
			return Redirect($"/festivals/{festivalId}?toast=access-denied");
		}

		// Fetch film metadata for the view
		var festivalFilm = await _festivalFilmService.GetByIdAsync(festivalFilmId, cancellationToken);
		if (festivalFilm == null)
			return NotFound();

		var film = await _filmService.GetByIdAsync(festivalFilm.FilmId, cancellationToken);
		var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);

		// Demo YouTube video ID (matches the hardcoded embed already used in SessionController)
		var youtubeVideoId = "zSWdZVtXT7E";

		ViewBag.FestivalId = festivalId;
		ViewBag.FestivalFilmId = festivalFilmId;
		ViewBag.SessionId = sessionId;
		ViewBag.YoutubeVideoId = youtubeVideoId;
		ViewBag.FilmName = film?.Name ?? "Film";
		ViewBag.FilmDescription = film?.Description;
		ViewBag.PosterUrl = film?.ImageUrl;
		ViewBag.FestivalName = festival?.Name ?? "Festival";
		ViewBag.DisplayName = User.FindFirst(ClaimTypes.Name)?.Value
			?? User.FindFirst("name")?.Value
			?? "Guest";
		ViewBag.JoinCode = HttpContext.Request.Query["joinCode"].FirstOrDefault() ?? "";

		return View();
	}
}
