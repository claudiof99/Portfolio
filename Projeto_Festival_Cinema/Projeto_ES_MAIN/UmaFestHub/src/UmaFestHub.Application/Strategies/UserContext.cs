using UmaFestHub.Application.Enums;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;
namespace UmaFestHub.Application.Strategies;
public class UserContext
{
    private readonly IUserRoleStrategy _strategy;

    public UserContext(IEnumerable<IUserRoleStrategy> strategies, string role)
    {
        _strategy = strategies.FirstOrDefault(s => s.Role == role)
            ?? new CustomerStrategy();
    }

    public RedirectDestination GetRedirectDestination() => _strategy.GetRedirectDestination();
}