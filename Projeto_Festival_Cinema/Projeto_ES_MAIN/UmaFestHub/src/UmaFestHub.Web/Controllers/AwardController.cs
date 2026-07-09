// -----------------------------------------------------------------------------
// Awards, nominations & votes — Organizer UI: list/filter awards, create award, GET/POST
// nominees (four distinct picks); server-side validation messages.
// -----------------------------------------------------------------------------
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = "Organizer,Admin")]
[Route("awards")]
public class AwardController : Controller
{
	private readonly IAwardService _awardService;
	private readonly IFestivalService _festivalService;
	private readonly INominationCandidatesService _nominationCandidatesService;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public AwardController(
		IAwardService awardService,
		IFestivalService festivalService,
		INominationCandidatesService nominationCandidatesService,
		IStringLocalizer<SharedResources> localizer)
	{
		_awardService = awardService;
		_festivalService = festivalService;
		_nominationCandidatesService = nominationCandidatesService;
		_localizer = localizer;
	}

	[HttpGet("", Name = "AwardIndex")]
	public async Task<IActionResult> Index([FromQuery] Guid? festivalId, int page = 1, CancellationToken cancellationToken = default)
	{
		const int awardPageSize = 3;
		var festivals = await _festivalService.GetAllAsync(cancellationToken);
		var festivalOptions = festivals
			.Select(f => new FestivalOptionViewModel { Id = f.Id, Name = f.Name })
			.ToList();

		Guid? selectedFestivalId = festivalId is { } fid && fid != Guid.Empty ? fid : null;

		var (awards, hasNext) = await _awardService.GetPageAsync(page, awardPageSize, cancellationToken);

		return View(new AwardIndexPageViewModel
		{
			Page = page < 1 ? 1 : page,
			HasNext = hasNext,
			FestivalId = selectedFestivalId,
			Festivals = festivalOptions,
			Awards = awards.Select(x => x.ToViewModel()).ToList()
		});
	}

	[HttpGet("nominees", Name = "AwardNominees")]
	public async Task<IActionResult> Nominees(
		Guid festivalId,
		int category,
		string? awardName = null,
		string? endDate = null,
		string categoryLabel = "",
		CancellationToken cancellationToken = default)
	{
		if (festivalId == Guid.Empty)
		{
			TempData["AwardCreateError"] = _localizer["Award_FestivalRequired"].Value;
			return RedirectToAction(nameof(Index));
		}

		if (string.IsNullOrWhiteSpace(awardName))
		{
			TempData["AwardCreateError"] = _localizer["Award_NameRequired"].Value;
			return RedirectToAwardIndex(festivalId);
		}

		if (!TryParseAwardEndDateUtc(endDate, out var endDateUtc, out var endDateErrorKey))
		{
			TempData["AwardCreateError"] = _localizer[endDateErrorKey!].Value;
			return RedirectToAwardIndex(festivalId);
		}

		var parsed = Enum.IsDefined(typeof(AwardCategory), category)
			? (AwardCategory)category
			: AwardCategory.Film;

		var displayCategory = DisplayCategoryLabel(categoryLabel, parsed);
		var vm = await BuildNomineesPageViewModelAsync(
			festivalId,
			parsed,
			awardName,
			displayCategory,
			endDate!,
			endDateUtc,
			cancellationToken);
		return View("~/Views/Award/Nominees.cshtml", vm);
	}

	[HttpPost("nominees", Name = "AwardSaveNominees")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> SaveNominees(
		Guid festivalId,
		int categoryValue,
		string? awardName,
		string? categoryLabel,
		string? endDate,
		Guid nominee1,
		Guid nominee2,
		Guid nominee3,
		Guid nominee4,
		CancellationToken cancellationToken)
	{
		if (festivalId == Guid.Empty)
		{
			return RedirectToAction(nameof(Index));
		}

		if (!Enum.IsDefined(typeof(AwardCategory), categoryValue))
		{
			return RedirectToAwardIndex(festivalId);
		}

		var category = (AwardCategory)categoryValue;
		var nomineeIds = new[] { nominee1, nominee2, nominee3, nominee4 };

		if (!TryParseAwardEndDateUtc(endDate, out var endDateUtc, out var endDateErrorKey))
		{
			TempData["AwardCreateError"] = _localizer[endDateErrorKey!].Value;
			return RedirectToAwardIndex(festivalId);
		}

		if (nomineeIds.Any(id => id == Guid.Empty))
		{
			var vmEmpty = await BuildNomineesPageViewModelAsync(
				festivalId,
				category,
				awardName,
				DisplayCategoryLabel(categoryLabel, category),
				endDate ?? string.Empty,
				endDateUtc,
				cancellationToken,
				errorMessage: _localizer["Award_SelectFourNominees"].Value,
				selectedNominee1: nominee1 == Guid.Empty ? null : nominee1,
				selectedNominee2: nominee2 == Guid.Empty ? null : nominee2,
				selectedNominee3: nominee3 == Guid.Empty ? null : nominee3,
				selectedNominee4: nominee4 == Guid.Empty ? null : nominee4);
			return View("~/Views/Award/Nominees.cshtml", vmEmpty);
		}

		if (nomineeIds.Distinct().Count() != nomineeIds.Length)
		{
			var vmDup = await BuildNomineesPageViewModelAsync(
				festivalId,
				category,
				awardName,
				DisplayCategoryLabel(categoryLabel, category),
				endDate ?? string.Empty,
				endDateUtc,
				cancellationToken,
				errorMessage: _localizer["Award_RepeatedNominees"].Value,
				selectedNominee1: nominee1,
				selectedNominee2: nominee2,
				selectedNominee3: nominee3,
				selectedNominee4: nominee4);
			return View("~/Views/Award/Nominees.cshtml", vmDup);
		}

		var candidates = await _nominationCandidatesService.GetCandidatesAsync(category, festivalId, cancellationToken);
		var distinctCandidateCount = candidates.Select(c => c.Id).Distinct().Count();
		if (distinctCandidateCount < 4)
		{
			var vmTooFew = await BuildNomineesPageViewModelAsync(
				festivalId,
				category,
				awardName,
				DisplayCategoryLabel(categoryLabel, category),
				endDate ?? string.Empty,
				endDateUtc,
				cancellationToken,
				errorMessage: _localizer["Award_InsufficientOptions"].Value);
			return View("~/Views/Award/Nominees.cshtml", vmTooFew);
		}

		try
		{
			await _awardService.CreateWithNomineesAsync(
				festivalId,
				awardName ?? string.Empty,
				category,
				nomineeIds,
				endDateUtc,
				cancellationToken);
		}
		catch (InvalidOperationException ex)
		{
			var vmError = await BuildNomineesPageViewModelAsync(
				festivalId,
				category,
				awardName,
				DisplayCategoryLabel(categoryLabel, category),
				endDate ?? string.Empty,
				endDateUtc,
				cancellationToken,
				errorMessage: _localizer.LocalizeKeyOrFallback(ex.Message),
				selectedNominee1: nominee1,
				selectedNominee2: nominee2,
				selectedNominee3: nominee3,
				selectedNominee4: nominee4);
			return View("~/Views/Award/Nominees.cshtml", vmError);
		}
		catch (ArgumentException)
		{
			TempData["AwardCreateError"] = _localizer["Award_EndDateInvalid"].Value;
			return RedirectToAwardIndex(festivalId);
		}

		return RedirectToAwardIndex(festivalId);
	}

	private static bool TryParseAwardEndDateUtc(string? endDate, out DateTime endDateUtc, out string? errorKey)
	{
		endDateUtc = default;
		errorKey = null;

		if (string.IsNullOrWhiteSpace(endDate))
		{
			errorKey = "Award_EndDateRequired";
			return false;
		}

		if (!DateTime.TryParse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
			&& !DateTime.TryParse(endDate, out parsedDate))
		{
			errorKey = "Award_EndDateInvalid";
			return false;
		}

		endDateUtc = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 23, 59, 59, DateTimeKind.Utc);
		if (endDateUtc <= DateTime.UtcNow)
		{
			errorKey = "Award_EndDateMustBeFuture";
			return false;
		}

		return true;
	}

	private string DisplayCategoryLabel(string? categoryLabel, AwardCategory category)
		=> string.IsNullOrWhiteSpace(categoryLabel)
			? _localizer.LocalizeAwardCategory(category.ToString())
			: categoryLabel;

	private async Task<AwardNomineesPageViewModel> BuildNomineesPageViewModelAsync(
		Guid festivalId,
		AwardCategory category,
		string? awardName,
		string categoryDisplay,
		string endDate,
		DateTime endDateUtc,
		CancellationToken cancellationToken,
		string? errorMessage = null,
		Guid? selectedNominee1 = null,
		Guid? selectedNominee2 = null,
		Guid? selectedNominee3 = null,
		Guid? selectedNominee4 = null)
	{
		var options = await _nominationCandidatesService.GetCandidatesAsync(category, festivalId, cancellationToken);
		return new AwardNomineesPageViewModel
		{
			FestivalId = festivalId,
			AwardName = awardName,
			Category = categoryDisplay,
			CategoryValue = (int)category,
			EndDate = endDate,
			EndDateUtc = endDateUtc,
			Options = options
				.Select(o => new NomineeOptionViewModel { Id = o.Id, Label = o.Label, ImageUrl = o.ImageUrl })
				.ToList(),
			ErrorMessage = errorMessage,
			SelectedNominee1 = selectedNominee1,
			SelectedNominee2 = selectedNominee2,
			SelectedNominee3 = selectedNominee3,
			SelectedNominee4 = selectedNominee4,
		};
	}

	[HttpPost("create", Name = "AwardCreate")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(AwardDto model, CancellationToken cancellationToken)
	{
		await _awardService.CreateAsync(model, cancellationToken);
		return RedirectToAwardIndex(model.FestivalId);
	}

	[HttpPost("{awardId:guid}/nominate", Name = "AwardNominate")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Nominate(Guid awardId, Guid festivalId, Guid festivalFilmId, CancellationToken cancellationToken)
	{
		await _awardService.NominateAsync(awardId, festivalFilmId, cancellationToken);
		return RedirectToAwardIndex(festivalId);
	}

	private IActionResult RedirectToAwardIndex(Guid festivalId, int page)
	{
		page = page < 1 ? 1 : page;
		if (festivalId == Guid.Empty)
			return RedirectToAction(nameof(Index), new { page });
		return RedirectToAction(nameof(Index), new { festivalId, page });
	}

	private IActionResult RedirectToAwardIndex(Guid festivalId)
	{
		if (festivalId == Guid.Empty)
			return RedirectToAction(nameof(Index));
		return RedirectToAction(nameof(Index), new { festivalId });
	}
}
