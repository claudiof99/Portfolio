using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Factories;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = "Organizer,Admin")]
[Route("tickets")]
public class TicketController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFactory _productFactory;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public TicketController(
        IProductRepository productRepository,
        IProductFactory productFactory,
        IStringLocalizer<SharedResources> localizer)
    {
        _productRepository = productRepository;
        _productFactory = productFactory;
        _localizer = localizer;
    }

    [HttpGet("create")]
    public IActionResult Create(Guid sessionId)
    {
        var model = new CreateTicketViewModel { SessionId = sessionId, Price = 10.00m };
        return View(model);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTicketViewModel model, CancellationToken cancellationToken)
    {
        var priceStr = Request.Form["Price"].ToString().Replace(",", ".");
        ModelState.Clear();

        if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedPrice) && parsedPrice >= 0)
        {
            model.Price = parsedPrice;
        }
        else
        {
            TempData["ErrorMessage"] = _localizer["Common_InvalidPrice"].Value;
            return View(model);
        }

        var product = _productFactory.Create(
            productType: "Ticket",
            price: model.Price,
            sessionId: model.SessionId);

        var productWithId = CreateProductWithId(product);

        await _productRepository.AddAsync(productWithId, cancellationToken);
        return RedirectToAction("Index", "Session", new { festivalFilmId = Guid.Empty });
    }

    private static Domain.Entities.Product CreateProductWithId(Domain.Entities.Product product)
    {
        return product switch
        {
            Domain.Entities.Ticket t => new Domain.Entities.Ticket(t.SessionId, t.Price) { Id = Guid.NewGuid() },
            Domain.Entities.DailyPass dp => new Domain.Entities.DailyPass(dp.FestivalId, dp.Price, dp.DateUtc) { Id = Guid.NewGuid() },
            Domain.Entities.CompletePass cp => new Domain.Entities.CompletePass(cp.FestivalId, cp.Price) { Id = Guid.NewGuid() },
            Domain.Entities.Rental r => new Domain.Entities.Rental(r.FestivalFilmId, r.Price, r.Duration) { Id = Guid.NewGuid() },
            _ => throw new ArgumentException($"Unknown product type: {product.GetType().Name}")
        };
    }
}