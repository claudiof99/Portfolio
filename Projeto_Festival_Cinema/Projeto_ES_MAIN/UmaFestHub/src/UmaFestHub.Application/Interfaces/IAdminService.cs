using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IAdminService
{
	Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
	Task<AdminDashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
	Task DeleteUserAsync(Guid actingAdminId, Guid targetUserId, CancellationToken cancellationToken = default);
}
