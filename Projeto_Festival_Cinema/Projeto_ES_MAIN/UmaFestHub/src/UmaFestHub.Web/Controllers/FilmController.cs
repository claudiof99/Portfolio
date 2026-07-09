using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Web.Resources;using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Mappings;

using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;
[Route("films")]
public class FilmController : Controller
{
	private readonly IFilmService _filmService;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public FilmController(IFilmService filmService, IStringLocalizer<SharedResources> localizer)
	{
		_filmService = filmService;
		_localizer = localizer;
	}

	[HttpGet("", Name = "FilmIndex")]
	public async Task<IActionResult> Index(string? title, string? genre, int? minDurationMinutes, int? maxDurationMinutes, CancellationToken cancellationToken)
	{
		var hasFilters = !string.IsNullOrWhiteSpace(title)
			|| !string.IsNullOrWhiteSpace(genre)
			|| minDurationMinutes.HasValue
			|| maxDurationMinutes.HasValue;

		var films = hasFilters
			? await _filmService.SearchAsync(title, genre, minDurationMinutes, maxDurationMinutes, cancellationToken)
			: await _filmService.GetAllAsync(cancellationToken);

		var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
		films = await _filmService.LocalizeFilmsAsync(films, tmdbLanguage, cancellationToken);

		var filmViewModels = films.Select(x => 
		{
			var vm = x.ToViewModel();
			vm.Credits = x.Credits;
			return vm;
		}).ToList();

		ViewBag.FilterTitle = title;
		ViewBag.FilterGenre = genre;
		ViewBag.FilterMinDurationMinutes = minDurationMinutes;
		ViewBag.FilterMaxDurationMinutes = maxDurationMinutes;
		return View(filmViewModels);
	}

	[HttpGet("/films/{id:guid}", Name = "FilmDetails")]
	public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
	{
		if (id == Guid.Empty)
		{
			return View();
		}

		var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
		var film = await _filmService.GetByIdLocalizedAsync(id, tmdbLanguage, cancellationToken);
		if (film is null)
		{
			return View((FilmViewModel?)null);
		}

		var vm = film.ToViewModel();
		vm.Credits = film.Credits;
		return View(vm);
	}

	[HttpPost("/edit", Name = "FilmEdit")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(FilmDto model, CancellationToken cancellationToken)
	{
		await _filmService.CreateAsync(model, cancellationToken);
		return RedirectToRoute("FilmIndex");
	}

	[Authorize(Roles = "Admin")]
	[HttpPost("Delete/{id:guid}", Name = "FilmDelete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			await _filmService.DeleteAsync(id, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		catch (DbUpdateException)
		{
			TempData["ErrorMessage"] = _localizer["Film_CannotDelete"].Value;
			return RedirectToAction(nameof(Details), new { id });
		}
	}
}
