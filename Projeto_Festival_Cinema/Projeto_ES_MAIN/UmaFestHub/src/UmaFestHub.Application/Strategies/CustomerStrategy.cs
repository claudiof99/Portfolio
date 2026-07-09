using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Enums;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;

public class CustomerStrategy : IUserRoleStrategy
{
    public string Role => UserRole.Customer.ToString();
    public RedirectDestination GetRedirectDestination() => RedirectDestination.Home;

}