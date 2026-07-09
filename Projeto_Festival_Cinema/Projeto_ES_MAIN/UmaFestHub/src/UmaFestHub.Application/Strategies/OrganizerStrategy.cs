using UmaFestHub.Application.Enums;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;

public class OrganizerStrategy : IUserRoleStrategy
{
    public string Role => UserRole.Organizer.ToString();
    public RedirectDestination GetRedirectDestination() => RedirectDestination.OrganizerDashboard;
}