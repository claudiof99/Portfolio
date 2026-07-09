using UmaFestHub.Application.Enums;
using UmaFestHub.Web.Constants;


public static class RedirectDestinationMapper
{
    public static string ToRouteName(RedirectDestination destination) => destination switch
    {
        RedirectDestination.AdminDashboard => RouteNames.AdminIndex,
        RedirectDestination.OrganizerDashboard => RouteNames.ManageIndex,
        RedirectDestination.Home => RouteNames.Home,
        _ => RouteNames.Home
    };
}