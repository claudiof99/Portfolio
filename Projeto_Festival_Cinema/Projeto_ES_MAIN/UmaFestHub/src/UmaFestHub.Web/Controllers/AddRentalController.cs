using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Controllers;

[Authorize]
[Route("rentals")]
public class AddRentalController : Controller
{
	private readonly IProductService _productService;
	private readonly ICartService _cartService;
	private readonly IStringLocalizer<SharedResources> _localizer;

	public AddRentalController(
		IProductService productService,
		ICartService cartService,
		IStringLocalizer<SharedResources> localizer)
	{
		_productService = productService;
		_cartService = cartService;
		_localizer = localizer;
	}

	[HttpGet("add/{festivalFilmId:guid}", Name = "AddRentalIndex")]
	public async Task<IActionResult> Add(Guid festivalFilmId, string? returnUrl, CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
			return Challenge();

		var rental = await _productService.GetRentalDtoAsync(festivalFilmId, cancellationToken);
		if (rental == null)
		{
			return NotFound(_localizer["SessionAccess_NoRentalProduct"].Value);
		}

		try
		{
			await _cartService.AddProductAsync(userId, rental.Id, 1, cancellationToken);
			return RedirectToAction("Index", "Cart", new { returnUrl });
		}
		catch (UserFacingException ex)
		{
			TempData["DuplicateCartItemMessage"] = _localizer.LocalizeUserFacing(ex);
			return RedirectToAction("Index", "Cart", new { returnUrl });
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = _localizer["Cart_AddRentalFailed"].Value;
			return RedirectToAction("Index", "Cart", new { returnUrl });
		}
	}
}