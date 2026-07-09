using UmaFestHub.Application.Enums;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces;

public interface IUserRoleStrategy
{
    string Role { get; }
    RedirectDestination GetRedirectDestination();
    
}