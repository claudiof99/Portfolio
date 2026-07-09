using System.ComponentModel.Design;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.ViewModels;
using System.Security.Claims;
using System.Reflection.PortableExecutable;
using System.Net.Http.Headers;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = "Organizer")]
[Route("rentals")]
public sealed class RentalController : Controller
{
    private readonly IProductService _productService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public RentalController(IProductService productService, IStringLocalizer<SharedResources> localizer)
    {
        _productService = productService;
        _localizer = localizer;
    }

    [HttpGet("create", Name = "RentalCreateGet")]
    public IActionResult Create(Guid festivalFilmId)
    {
        var model = new CreateRentalViewModel
        {
            FestivalFilmId = festivalFilmId,
            Price = 5.99m,
            DurationValue = 48,
            DurationUnit = "Hours"
        };
        return View(model);
    }

    [HttpPost("create", Name = "RentalCreatePost")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateRentalViewModel model,
        CancellationToken cancellationToken)
    {
        // Manually parse the price to prevent silent failures caused by European comma/dot decimal formats
        var priceStr = Request.Form["Price"].ToString().Replace(",", ".");
        ModelState.Clear(); // Clear strict binding errors

        if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedPrice) && parsedPrice >= 0)
        {
            model.Price = parsedPrice;
        }
        else
        {
            TempData["ErrorMessage"] = _localizer["Common_InvalidPrice"].Value;
            return View(model);
        }

        if (model.FestivalFilmId == Guid.Empty || model.DurationValue <= 0 || string.IsNullOrWhiteSpace(model.DurationUnit))
        {
            TempData["ErrorMessage"] = _localizer["Rental_MissingInfo"].Value;
            return View(model);
        }

        var rentalDto = new RentalDto(
            Id: Guid.Empty,
            ProductType: "Rental",
            Price: model.Price,
            FestivalFilmId: model.FestivalFilmId,
            DurationValue: model.DurationValue,
            DurationUnit: model.DurationUnit);

        await _productService.CreateRentalAsync(rentalDto, cancellationToken);

        return RedirectToAction("Details", "FestivalFilm", new { id = model.FestivalFilmId });
    }
}
