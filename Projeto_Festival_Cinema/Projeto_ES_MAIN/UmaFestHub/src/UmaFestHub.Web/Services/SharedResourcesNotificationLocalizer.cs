using Microsoft.Extensions.Localization;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Services;

public sealed class SharedResourcesNotificationLocalizer(IStringLocalizer<SharedResources> localizer) : INotificationLocalizer
{
	public string Get(string key, params object[] args)
		=> args.Length == 0 ? localizer[key].Value : localizer[key, args].Value;
}
