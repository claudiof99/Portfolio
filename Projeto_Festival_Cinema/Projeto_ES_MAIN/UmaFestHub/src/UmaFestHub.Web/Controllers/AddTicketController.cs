using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.Security;

namespace UmaFestHub.Web.Controllers;

[Authorize]
[Route("tickets/add")]
public class AddTicketController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public AddTicketController(
        IProductService productService,
        ICartService cartService,
        IStringLocalizer<SharedResources> localizer)
    {
        _productService = productService;
        _cartService = cartService;
        _localizer = localizer;
    }

    [HttpGet("", Name = "AddTicketIndex")]
    public async Task<IActionResult> Index(Guid sessionId, string? returnUrl, CancellationToken cancellationToken)
    {
        // Resolves the ticket product assigned to this Session
        var ticket = await _productService.GetTicketDtoAsync(sessionId, cancellationToken);
        if (ticket == null)
        {
            return NotFound(_localizer["AddTicket_NotConfigured"].Value);
        }

        if (User.TryGetCurrentUserId(out var userId))
        {
            await _cartService.AddProductAsync(userId, ticket.Id, 1, cancellationToken);
        }

        return RedirectToAction("Index", "Cart", new { returnUrl });
    }
}
