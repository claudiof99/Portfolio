using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Exceptions;

namespace UmaFestHub.Application.Security
{
    public static class PermissionGuard
    {
        public static void EnsureRole(User? user, UserRole requiredRole)
        {
            if (user == null || !user.Roles.Contains(requiredRole))
            {
                throw new UnauthorizedException($"User does not have the required '{requiredRole}' role to perform this action.");
            }
        }
    }
}