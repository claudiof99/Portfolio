using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces
{
    public interface IUserRoleService
    {
        Task PromoteUserAsync(User actingAdmin, User targetUser, UserRole newRole, CancellationToken cancellationToken = default);
        Task DemoteUserAsync(User actingAdmin, User targetUser, UserRole roleToRemove, CancellationToken cancellationToken = default);
    }
}