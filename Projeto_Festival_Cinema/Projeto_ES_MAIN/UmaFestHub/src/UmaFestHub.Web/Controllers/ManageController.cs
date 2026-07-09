using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.Security;

namespace UmaFestHub.Web.Controllers;

public class ManageController : Controller
{
    private readonly IFestivalService _festivalService;

    public ManageController(IFestivalService festivalService)
    {
        _festivalService = festivalService;
    }

    [Authorize(Roles = RoleConstants.ModeratorRolesCsv)]
    [HttpGet("/manage", Name = "ManageIndex")]
	public IActionResult Index() => View("~/Views/Manage/Index.cshtml");

    [Authorize(Roles = RoleConstants.OrganizerOrAdminRolesCsv)]
    [HttpGet("/manage/festivals/{festivalId:guid}/import", Name = "ManageFestivalImportFilm")]
    public async Task<IActionResult> ImportFilm(Guid festivalId, CancellationToken ct)
    {
        var festival = await _festivalService.GetByIdAsync(festivalId, ct);
        
        if (festival is null) 
        {
            return NotFound();
        }
        
        ViewBag.FestivalId = festivalId;
        ViewBag.FestivalName = festival.Name;
        
        return View("~/Views/Manage/ImportFilm.cshtml");
    }

    [HttpGet("/manage/festivals", Name = "ManageFestivals")]
	public async Task<IActionResult> Festivals(CancellationToken cancellationToken)
	{
		var festivals = await _festivalService.GetAllAsync(cancellationToken);
		var viewModels = festivals.Select(x => x.ToViewModel()).ToList();
		return View("~/Views/Manage/Festivals.cshtml", viewModels);
	}

	[HttpGet("/manage/films", Name = "ManageFilms")]
	public IActionResult Films() => RedirectToRoute("FilmIndex");

	[HttpGet("/manage/reviews", Name = "ManageReviews")]
	public IActionResult Reviews() => RedirectToRoute("ReviewManage");

	[HttpGet("/manage/awards", Name = "ManageAwards")]
	public async Task<IActionResult> Awards(CancellationToken cancellationToken = default)
	{
		var festivals = await _festivalService.GetAllAsync(cancellationToken);
		if (festivals.Count == 0)
		{
			return RedirectToRoute("ManageFestivals");
		}

		return RedirectToRoute("AwardIndex");
	}

	[HttpGet("/manage/sessions", Name = "ManageSessions")]
	public IActionResult Sessions() => RedirectToRoute("ManageFestivals", new { festivalFilmId = Guid.Empty });
}
