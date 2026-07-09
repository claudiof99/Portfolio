using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Web.Controllers;

[Authorize]
public class WatchController : Controller
{
    private readonly IEntitlementService _entitlementService;

    public WatchController(IEntitlementService entitlementService)
    {
        _entitlementService = entitlementService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var userId))
        {
            return Challenge();
        }

        var hasAccess = await _entitlementService.CanWatchMovieAsync(userId, festivalId, festivalFilmId, sessionId, cancellationToken);
        if (!hasAccess)
        {
            return View("AccessDenied");
        }

        return View("Watch", "https://www.w3schools.com/html/mov_bbb.mp4");
    }
}