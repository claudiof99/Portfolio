using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.Pricing;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.ViewModels;
using System.Security.Claims;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Web.Extensions;

namespace UmaFestHub.Web.Controllers;

[Authorize]
[Route("cart")]
public class AddPassController : Controller
{
	private readonly ICartService _cartService;
	private readonly IFestivalService _festivalService;
	private readonly IProductService _productService;
	private readonly IPricingService _pricingService;
	private readonly IProductRepository _productRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public AddPassController(
		ICartService cartService,
		IFestivalService festivalService,
		IProductService productService,
		IPricingService pricingService,
		IProductRepository productRepository,
		IStringLocalizer<SharedResources> localizer)
	{
		_cartService = cartService;
		_festivalService = festivalService;
		_productService = productService;
		_pricingService = pricingService;
		_productRepository = productRepository;
		_localizer = localizer;
	}

	[HttpGet("add-pass/{festivalId:guid}", Name = "AddPassIndex")]
	public async Task<IActionResult> Index(
		Guid festivalId,
		string type,
		string? returnUrl,
		CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId)) return Challenge();

		var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);
		if (festival is null)
			return NotFound();

		decimal price = 0m;
		string passType = type == "Day" ? "DailyPass" : "CompletePass";
		Guid productId = Guid.Empty;

		if (passType == "DailyPass")
		{
			var product = await _productService.GetDailyPassDtoAsync(festivalId, cancellationToken);
			if (product is null)
			{
				return Content(_localizer["AddPass_DailyNotConfigured"].Value);
			}
			productId = product.Id;
		}
		else
		{
			var product = await _productService.GetCompletePassDtoAsync(festivalId, cancellationToken);
			if (product is null)
			{
				return Content(_localizer["AddPass_CompleteNotConfigured"].Value);
			}
			productId = product.Id;
		}

		var entity = await _productRepository.GetByIdAsync(productId, cancellationToken);
		if (entity != null)
		{
			price = await _pricingService.GetPriceAsync(entity, userId, DateTime.UtcNow, cancellationToken);
		}

		ViewData["festivalName"] = festival.Name;
		ViewData["passType"] = type == "Day" ? _localizer["Pass_DailyPass"].Value : _localizer["Pass_CompletePass"].Value;
		ViewData["festivalId"] = festivalId;
		ViewData["type"] = type;
		ViewData["returnUrl"] = ResolveCheckoutReturnUrl(festivalId, returnUrl);

		return View(new CartItemViewModel { Price = price });
	}

	[HttpPost("add-pass/{festivalId:guid}", Name = "AddPassPost")]
	public async Task<IActionResult> Add(
		Guid festivalId,
		string type,
		string? returnUrl,
		CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
			return Challenge();

		try
		{
			string passType = type == "Day" ? "DailyPass" : "CompletePass";

			if (passType == "DailyPass")
			{
				var product = await _productService.GetDailyPassDtoAsync(festivalId, cancellationToken);
				if (product is null)
					return NotFound();

				await _cartService.AddProductAsync(userId, product.Id, 1, cancellationToken);
			}
			else
			{
				var product = await _productService.GetCompletePassDtoAsync(festivalId, cancellationToken);
				if (product is null)
					return NotFound();

				await _cartService.AddProductAsync(userId, product.Id, 1, cancellationToken);
			}

			return RedirectToAction("Index", "Cart", new { returnUrl = ResolveCheckoutReturnUrl(festivalId, returnUrl) });
		}
		catch (UserFacingException ex)
		{
			TempData["DuplicateCartItemMessage"] = _localizer.LocalizeUserFacing(ex);
			return RedirectToAction("Index", "Cart", new { returnUrl = ResolveCheckoutReturnUrl(festivalId, returnUrl) });
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = _localizer["Cart_AddFailed"].Value;
			return RedirectToAction("Index", "Cart", new { returnUrl = ResolveCheckoutReturnUrl(festivalId, returnUrl) });
		}
	}

	private string? ResolveCheckoutReturnUrl(Guid festivalId, string? returnUrl)
	{
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return returnUrl;
		}

		return Url.RouteUrl("FestivalDetails", new { id = festivalId });
	}

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var value = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return Guid.TryParse(value, out userId);
	}
}