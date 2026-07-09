using System.Security.Cryptography;
using System.Text;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.Enums; 

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to handle user registration, secure authentication, and password reset flows.
/// </summary>
public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;

	public UserService(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	/// <summary>
	/// We register a new user, securely hashing their password and automatically assigning roles based on their email.
	/// </summary>
	public async Task<(bool Succeeded, string? Error)> RegisterAsync(string email, string password, string fullName, bool asAdmin, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			return (false, "Account_EmailPasswordRequired");
		}

		if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
		{
			return (false, "Account_UserAlreadyExists");
		}

		var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
		var hash = HashPassword(password, salt);

		var role = UserRole.Customer;
		if (asAdmin || email.Contains("admin", StringComparison.OrdinalIgnoreCase)) role = UserRole.Admin;
		else if (email.Contains("organizer", StringComparison.OrdinalIgnoreCase)) role = UserRole.Organizer;

		User user = new User();

		user.Id = Guid.NewGuid();
		user.Email = email.Trim();
		user.Name = string.IsNullOrWhiteSpace(fullName) ? email.Trim() : fullName.Trim();
		user.PasswordSalt = salt;
		user.PasswordHash = hash;

		user.Roles = new List<UserRole> { role };

		await _userRepository.AddAsync(user, cancellationToken);
		return (true, null);
	}

	/// <summary>
	/// We authenticate a user by verifying their credentials against our securely stored password hashes.
	/// </summary>
	public async Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			return null;
		}

		var user = await _userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
		if (user is null)
		{
			return null;
		}

		var computed = HashPassword(password, user.PasswordSalt);
		if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computed), Encoding.UTF8.GetBytes(user.PasswordHash)))
		{
			return null;
		}

		return new UserDto(
			user.Id,
			user.Name,
			user.Email,
			user.Roles.Select(r => r.ToString()).ToList());
	}

	/// <summary>
	/// We generate a secure, temporary 6-digit code to allow users to reset their forgotten passwords.
	/// </summary>
	public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return null;
		}

		var user = await _userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
		if (user is null)
		{
			return null;
		}

		var token = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
		user.PasswordResetToken = HashResetToken(token);
		user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

		await _userRepository.UpdateAsync(user, cancellationToken);
		return token;
	}

	public async Task<UserDto?> UpdateUserProfileAsync(Guid userId, string? newName, string? newEmail, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
		if (user is null)
		{
			return null;
		}

		if (!string.IsNullOrWhiteSpace(newName))
		{
			user.Name = newName.Trim();
		}

		if (!string.IsNullOrWhiteSpace(newEmail))
		{
			user.Email = newEmail.Trim();
		}

		await _userRepository.UpdateAsync(user, cancellationToken);

		return new UserDto(
			user.Id,
			user.Name,
			user.Email,
			user.Roles.Select(r => r.ToString()).ToList());
	}

	/// <summary>
	/// We validate the reset token and apply the user's new password if everything checks out.
	/// </summary>
	public async Task<(bool Succeeded, string? Error)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
		{
			return (false, "Account_ResetFieldsRequired");
		}

		var user = await _userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
		if (user is null)
		{
			return (false, "Account_EmailNotFound");
		}

		if (string.IsNullOrWhiteSpace(user.PasswordResetToken) || user.PasswordResetTokenExpiry is null)
		{
			return (false, "Account_ResetTokenInvalidOrExpired");
		}

		if (user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
		{
			return (false, "Account_ResetTokenExpired");
		}

		var providedTokenHash = HashResetToken(token.Trim());
		if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(providedTokenHash), Encoding.UTF8.GetBytes(user.PasswordResetToken)))
		{
			return (false, "Account_ResetTokenInvalidOrExpired");
		}

		var newSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
		user.PasswordSalt = newSalt;
		user.PasswordHash = HashPassword(newPassword, newSalt);
		user.PasswordResetToken = null;
		user.PasswordResetTokenExpiry = null;

		await _userRepository.UpdateAsync(user, cancellationToken);
		return (true, null);
	}

	/// <summary>
	/// We hash passwords using PBKDF2 with a unique salt to ensure secure storage.
	/// </summary>
	private static string HashPassword(string password, string salt)
    {
    var saltBytes = Convert.FromBase64String(salt);
    return Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
        password,
        saltBytes,
        100_000,
        HashAlgorithmName.SHA256,
        32));
    }

	/// <summary>
	/// We hash the reset tokens before storing them so that even database access doesn't reveal the raw codes.
	/// </summary>
	private static string HashResetToken(string token)
	{
		var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
		return Convert.ToBase64String(hashBytes);
	}
}
