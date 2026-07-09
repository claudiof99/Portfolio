using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Application.Security;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IFestivalService _festivalService;
        private readonly ISessionService _sessionService;

        public ManagerService(IFestivalService festivalService, ISessionService sessionService)
        {
            _festivalService = festivalService;
            _sessionService = sessionService;
        }

        public async Task CreateFestivalAsync(User user, string festivalName, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(user, UserRole.Organizer);
            // Route to actual festival creation logic
        }

        public async Task ManageSessionAsync(User user, Guid sessionId, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(user, UserRole.Organizer);
            // Route to actual session management logic
        }
    }
}
