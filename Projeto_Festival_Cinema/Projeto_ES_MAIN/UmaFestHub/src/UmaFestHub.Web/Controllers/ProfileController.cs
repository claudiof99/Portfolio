using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;


namespace UmaFestHub.Web.Controllers;

public class ProfileController : Controller
{
    [HttpGet("/profile")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
	{
		return View();
	}
}
