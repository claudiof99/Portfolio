using Microsoft.AspNetCore.Mvc;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Web.Controllers;

public class ErrorController : Controller
{
    public ErrorController()
	{
		
	}
	
    public IActionResult Index() => View("Error");
}
