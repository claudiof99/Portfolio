using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Mappings;

namespace UmaFestHub.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
	private readonly IAdminService _adminService;

	public AdminController(IAdminService adminService)
	{
		_adminService = adminService;
	}

	[HttpGet("", Name = "AdminIndex")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		var stats = await _adminService.GetDashboardStatsAsync(cancellationToken);
		return View(stats.ToViewModel());
	}
}
