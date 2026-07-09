using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IUserService
{
	Task<(bool Succeeded, string? Error)> RegisterAsync(string email, string password, string fullName, bool asAdmin, CancellationToken cancellationToken = default);
	Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
	Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
	Task<(bool Succeeded, string? Error)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

	Task<UserDto?> UpdateUserProfileAsync(Guid userId, string? newName, string? newEmail, CancellationToken cancellationToken = default);
}
