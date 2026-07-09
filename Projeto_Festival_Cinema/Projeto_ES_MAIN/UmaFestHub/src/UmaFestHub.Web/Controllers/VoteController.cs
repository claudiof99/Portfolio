// -----------------------------------------------------------------------------
// Awards, nominations & votes — Customer: list votable awards for a festival, POST one
// nomination per award; uses IAwardService (not IVoteService).
// -----------------------------------------------------------------------------
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Security;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = RoleConstants.CustomerRolesCsv)]
public sealed class VoteController : Controller
{
	private readonly IAwardService _awardService;
	private readonly IFestivalService _festivalService;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public VoteController(
		IAwardService awardService,
		IFestivalService festivalService,
		IStringLocalizer<SharedResources> localizer)
	{
		_awardService = awardService;
		_festivalService = festivalService;
		_localizer = localizer;
	}

	[HttpGet("/vote", Name = "VoteIndex")]
	public async Task<IActionResult> Index([FromQuery] Guid festivalId, CancellationToken cancellationToken)
	{
		if (festivalId == Guid.Empty)
		{
			return NotFound();
		}

		var userId = GetCurrentUserId();
		if (userId == Guid.Empty)
		{
			return Challenge();
		}

		var awards = await _awardService.GetByFestivalIdAvailableForVotingAsync(festivalId, userId, cancellationToken);
		var completedVotes = await _awardService.GetVotedAwardsForFestivalAsync(festivalId, userId, cancellationToken);

		var festivalName = awards.FirstOrDefault()?.FestivalName
			?? completedVotes.FirstOrDefault()?.Award.FestivalName
			?? string.Empty;
		if (string.IsNullOrWhiteSpace(festivalName))
		{
			var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);
			festivalName = festival?.Name ?? string.Empty;
		}

		var vm = new VoteIndexViewModel
		{
			FestivalId = festivalId,
			FestivalName = festivalName,
			Awards = awards,
			CompletedVotes = completedVotes,
			ErrorMessage = TempData["VoteError"] as string,
			SuccessMessage = TempData["VoteSuccess"] as string
		};

		return View(vm);
	}

	[HttpPost("/vote", Name = "VoteCast")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Cast([FromForm] Guid festivalId, CancellationToken cancellationToken)
	{
		if (festivalId == Guid.Empty)
		{
			return NotFound();
		}

		var userId = GetCurrentUserId();
		if (userId == Guid.Empty)
		{
			return Challenge();
		}

		var nominationIdValues = Request.Form["nominationId"];
		if (nominationIdValues.Count == 0)
		{
			TempData["VoteError"] = _localizer["Vote_SelectOneNominee"].Value;
			return RedirectToAction(nameof(Index), new { festivalId });
		}

		if (nominationIdValues.Count > 1)
		{
			TempData["VoteError"] = _localizer["Vote_OneAtATime"].Value;
			return RedirectToAction(nameof(Index), new { festivalId });
		}

		if (!Guid.TryParse(nominationIdValues[0], out var nominationId) || nominationId == Guid.Empty)
		{
			TempData["VoteError"] = _localizer["Vote_SelectOneNominee"].Value;
			return RedirectToAction(nameof(Index), new { festivalId });
		}

		try
		{
			await _awardService.VoteAsync(userId, nominationId, cancellationToken);
			TempData["VoteSuccess"] = _localizer["Vote_Submitted"].Value;
		}
		catch (UserFacingException ex)
		{
			TempData["VoteError"] = _localizer.LocalizeUserFacing(ex);
		}

		return RedirectToAction(nameof(Index), new { festivalId });
	}

	private Guid GetCurrentUserId()
	{
		var raw =
			User.FindFirstValue(ClaimTypes.NameIdentifier)
			?? User.FindFirstValue("sub")
			?? User.FindFirstValue("id");

		return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
	}
}
