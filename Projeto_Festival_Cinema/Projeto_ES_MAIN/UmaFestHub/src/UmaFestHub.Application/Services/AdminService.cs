using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Application.Security;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Services;

public class AdminService : IAdminService
{
	private readonly IFestivalRepository _festivalRepository;
	private readonly IFilmRepository _filmRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly IPurchaseRepository _purchaseRepository;
	private readonly IUserRepository _userRepository;

	public AdminService(
		IFestivalRepository festivalRepository,
		IFilmRepository filmRepository,
		ISessionRepository sessionRepository,
		IPurchaseRepository purchaseRepository,
		IUserRepository userRepository)
	{
		_festivalRepository = festivalRepository;
		_filmRepository = filmRepository;
		_sessionRepository = sessionRepository;
		_purchaseRepository = purchaseRepository;
		_userRepository = userRepository;
	}

	public Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
		=> Task.FromResult(true);

	public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
	{
		var festivals = await _festivalRepository.GetAllAsync(cancellationToken);
		var films = await _filmRepository.GetAllAsync(cancellationToken);
		var sessions = await _sessionRepository.GetAllAsync(cancellationToken);
		var purchases = await _purchaseRepository.CountAsync(cancellationToken);

		var now = DateTime.UtcNow;
		var activeSessions = sessions.Count(x => x.StartTimeUtc <= now && x.EndTimeUtc >= now);

		return new AdminDashboardStatsDto(
			festivals.Count,
			films.Count,
			activeSessions,
			purchases,
			true);
	}

	public async Task DeleteUserAsync(Guid actingAdminId, Guid targetUserId, CancellationToken cancellationToken = default)
	{
		var actingAdmin = await _userRepository.GetByIdAsync(actingAdminId, cancellationToken);
		if (actingAdmin == null) throw new UnauthorizedAccessException("Admin user not found.");

		PermissionGuard.EnsureRole(actingAdmin, UserRole.Admin);
		
		var targetUser = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
		if (targetUser != null)
		{
			await _userRepository.DeleteAsync(targetUser, cancellationToken);
		}
	}
}
