using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Enums;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;

public class AdminStrategy : IUserRoleStrategy
{
    public string Role => UserRole.Admin.ToString();
    public RedirectDestination GetRedirectDestination() => RedirectDestination.AdminDashboard;
}
