using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Pricing;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Controllers;

[Authorize]
public class PurchaseController : Controller
{
	private readonly IPurchaseService _purchaseService;
	private readonly ICartService _cartService;
	private readonly IPricingService _pricingService;
	private readonly IProductRepository _productRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly IFestivalFilmRepository _festivalFilmRepository;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public PurchaseController(
		IPurchaseService purchaseService,
		ICartService cartService,
		IPricingService pricingService,
		IProductRepository productRepository,
		ISessionRepository sessionRepository,
		IFestivalFilmRepository festivalFilmRepository,
		IStringLocalizer<SharedResources> localizer)
	{
		_purchaseService = purchaseService;
		_cartService = cartService;
		_pricingService = pricingService;
		_productRepository = productRepository;
		_sessionRepository = sessionRepository;
		_festivalFilmRepository = festivalFilmRepository;
		_localizer = localizer;
	}

	[HttpPost("/checkout")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Checkout(string? returnUrl, CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		var cart = await _cartService.GetByUserIdAsync(userId, cancellationToken);
		if (cart == null || !cart.Items.Any())
		{
			return RedirectToAction("Index", "Cart");
		}

		var purchaseDate = DateTime.UtcNow;
		var purchaseItems = new List<PurchaseItemDto>();

		foreach (var item in cart.Items)
		{
			var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
				?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

			var price = await _pricingService.GetPriceAsync(product, userId, purchaseDate, cancellationToken);
			purchaseItems.Add(new PurchaseItemDto(item.ProductId, item.Quantity, price));
		}

		try
		{
			await _purchaseService.CheckoutAsync(userId, purchaseItems, cancellationToken);
			TempData["SuccessMessage"] = _localizer["PurchaseHistory_SuccessMessage"].Value;
			TempData["ShowPurchaseConfetti"] = true;
			return await RedirectAfterCheckoutAsync(cart.Items.Select(x => x.ProductId), returnUrl, cancellationToken);
		}
		catch (UserFacingException ex)
		{
			TempData["ErrorMessage"] = _localizer.LocalizeUserFacing(ex);
			return RedirectToAction("Index", "Cart");
		}
	}

	[HttpGet("/history")]
	public async Task<IActionResult> History(CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Challenge();
		}

		var history = await _purchaseService.GetHistoryAsync(userId, cancellationToken);
		return View(history.Select(x => x.ToViewModel()).ToList());
	}

	private async Task<IActionResult> RedirectAfterCheckoutAsync(
		IEnumerable<Guid> productIds,
		string? returnUrl,
		CancellationToken cancellationToken)
	{
		if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
		{
			return LocalRedirect(returnUrl);
		}

		var festivalId = await ResolveFestivalIdFromProductsAsync(productIds, cancellationToken);
		if (festivalId.HasValue)
		{
			return RedirectToRoute("FestivalDetails", new { id = festivalId.Value });
		}

		return RedirectToAction("Index", "Cart");
	}

	private async Task<Guid?> ResolveFestivalIdFromProductsAsync(
		IEnumerable<Guid> productIds,
		CancellationToken cancellationToken)
	{
		Guid? resolvedFestivalId = null;

		foreach (var productId in productIds)
		{
			var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
			if (product is null)
			{
				continue;
			}

			var festivalId = product.GetFestivalId();
			if (!festivalId.HasValue && product is Ticket ticket)
			{
				var session = await _sessionRepository.GetByIdAsync(ticket.SessionId, cancellationToken);
				if (session is not null)
				{
					var festivalFilm = await _festivalFilmRepository.GetByIdAsync(session.FestivalFilmId, cancellationToken);
					festivalId = festivalFilm?.FestivalId;
				}
			}
			else if (!festivalId.HasValue && product is Rental rental)
			{
				var festivalFilm = await _festivalFilmRepository.GetByIdAsync(rental.FestivalFilmId, cancellationToken);
				festivalId = festivalFilm?.FestivalId;
			}

			if (!festivalId.HasValue)
			{
				continue;
			}

			if (resolvedFestivalId.HasValue && resolvedFestivalId.Value != festivalId.Value)
			{
				return resolvedFestivalId.Value;
			}

			resolvedFestivalId = festivalId.Value;
		}

		return resolvedFestivalId;
	}

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var value = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return Guid.TryParse(value, out userId);
	}
}