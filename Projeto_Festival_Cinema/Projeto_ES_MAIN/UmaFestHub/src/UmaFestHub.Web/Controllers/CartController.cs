using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Application.Pricing;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Controllers;

[Authorize]
public class CartController : Controller
{
	private readonly ICartService _cartService;
	private readonly IProductService _productService;
	private readonly IPricingService _pricingService;
	private readonly IProductRepository _productRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public CartController(
		ICartService cartService, 
		IProductService productService,
		IPricingService pricingService,
		IProductRepository productRepository,
		IStringLocalizer<SharedResources> localizer)
	{
		_cartService = cartService;
		_productService = productService;
		_pricingService = pricingService;
		_productRepository = productRepository;
		_localizer = localizer;
	}

	[HttpGet("/cart")]
	public async Task<IActionResult> Index(string? returnUrl, CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
			return Challenge();

		var cart = await _cartService.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null)
		{
			ViewBag.ReturnUrl = NormalizeReturnUrl(returnUrl);
			return View(null);
		}

		var vm = cart.ToViewModel();

		var enrichedItems = new List<CartItemViewModel>();
		foreach (var item in vm.Items)
		{
			var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);
			var productEntity = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
			
			var price = product?.Price ?? 0m;
			if (productEntity != null)
			{
				price = await _pricingService.GetPriceAsync(productEntity, userId, DateTime.UtcNow, cancellationToken);
			}

			enrichedItems.Add(new CartItemViewModel
			{
				Id = item.Id,
				ProductId = item.ProductId,
				Quantity = item.Quantity,
				ProductType = product?.ProductType != null
					? _localizer.LocalizeProductType(product.ProductType)
					: _localizer["Common_Unknown"].Value,
				Price = price
			});
		}

		vm.Items = enrichedItems;
		ViewBag.ReturnUrl = NormalizeReturnUrl(returnUrl);
		return View(vm);
	}

	[HttpPost("/cart/add")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Add(Guid productId, int quantity = 1, CancellationToken cancellationToken = default)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		try
		{
			await _cartService.AddProductAsync(userId, productId, quantity, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		catch (UserFacingException ex)
		{
			TempData["DuplicateCartItemMessage"] = _localizer.LocalizeUserFacing(ex);
			return RedirectToAction(nameof(Index));
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = _localizer["Cart_AddFailed"].Value;
			return RedirectToAction(nameof(Index));
		}
	}

	[HttpPost("/cart/remove")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Remove(Guid productId, string? returnUrl, CancellationToken cancellationToken = default)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		try
		{
			await _cartService.RemoveItemAsync(userId, productId, cancellationToken);
			return RedirectToAction(nameof(Index), new { returnUrl = NormalizeReturnUrl(returnUrl) });
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = _localizer["Cart_RemoveFailed"].Value;
			return RedirectToAction(nameof(Index));
		}
	}

	private string? NormalizeReturnUrl(string? returnUrl)
		=> !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var value = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
		return Guid.TryParse(value, out userId);
	}
}