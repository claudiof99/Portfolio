using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Security;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRepository _userRepository;

        public UserRoleService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task PromoteUserAsync(User actingAdmin, User targetUser, UserRole newRole, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(actingAdmin, UserRole.Admin);
            
            if (!targetUser.Roles.Contains(newRole))
                targetUser.Roles.Add(newRole);
                
            await _userRepository.UpdateAsync(targetUser, cancellationToken);
        }

        public async Task DemoteUserAsync(User actingAdmin, User targetUser, UserRole roleToRemove, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(actingAdmin, UserRole.Admin);
            
            if (targetUser.Roles.Contains(roleToRemove))
                targetUser.Roles.Remove(roleToRemove);
                
            await _userRepository.UpdateAsync(targetUser, cancellationToken);
        }
    }
}
