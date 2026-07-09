using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Security;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Application.Handlers;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
[Route("passes")]
public class PassController : Controller
{
	private readonly IProductRepository _productRepository;
	private readonly IPassDeleteHandler _passDeleteHandler;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public PassController(
		IProductRepository productRepository,
		IPassDeleteHandler passDeleteHandler,
		IStringLocalizer<SharedResources> localizer)
	{
		_productRepository = productRepository;
		_passDeleteHandler = passDeleteHandler;
		_localizer = localizer;
	}

	[HttpGet("create")]
	public async Task<IActionResult> Create([FromQuery] Guid festivalId, [FromQuery] string type, CancellationToken cancellationToken)
	{
		// Normalize type to match what we'll use in POST
		var normalizedType = NormalizePassType(type);

		// Try to get existing pass to show current price
		decimal currentPrice = 0m;
		if (normalizedType == "DailyPass")
		{
			var existing = await _productRepository.GetDailyPassAsync(festivalId, cancellationToken);
			currentPrice = existing?.Price ?? 0m;
		}
		else if (normalizedType == "CompletePass")
		{
			var existing = await _productRepository.GetCompletePassAsync(festivalId, cancellationToken);
			currentPrice = existing?.Price ?? 0m;
		}

		var model = new CreatePassViewModel
		{
			FestivalId = festivalId,
			PassType = normalizedType,
			Price = currentPrice
		};
		return View("CreatePass", model);
	}

	[HttpPost("create")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(CreatePassViewModel model, CancellationToken cancellationToken)
	{
		// Normalize PassType to match factory expectations
		model.PassType = NormalizePassType(model.PassType);

		var priceStr = Request.Form["Price"].ToString().Replace(",", ".");
		ModelState.Clear();

		if (!decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedPrice) || parsedPrice < 0)
		{
			TempData["ErrorMessage"] = _localizer["Common_InvalidPrice"].Value;
			return View("CreatePass", model);
		}

		model.Price = parsedPrice;

		if (model.FestivalId == Guid.Empty || string.IsNullOrEmpty(model.PassType))
		{
			TempData["ErrorMessage"] = _localizer["Pass_MissingInfo"].Value;
			return View("CreatePass", model);
		}

		// Validate pass type is recognized
		if (model.PassType != "DailyPass" && model.PassType != "CompletePass")
		{
			TempData["ErrorMessage"] = _localizer["Pass_UnknownType", model.PassType].Value;
			return View("CreatePass", model);
		}

		// Create or update pass with new price
		await _passDeleteHandler.CreateOrUpdateAsync(model.PassType, model.FestivalId, model.Price, cancellationToken);

		return Redirect($"/festivals/{model.FestivalId}");
	}

	private static string NormalizePassType(string? passType)
	{
		if (string.IsNullOrEmpty(passType))
			return string.Empty;

		return passType.Trim().ToLowerInvariant() switch
		{
			"day" => "DailyPass",
			"daily" => "DailyPass",
			"dailypass" => "DailyPass",
			"full" => "CompletePass",
			"complete" => "CompletePass",
			"completepass" => "CompletePass",
			_ => passType.Trim()
		};
	}
}