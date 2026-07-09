using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Domain.Interfaces;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Guid>> SearchIdsByNameAsync(string nameQuery, CancellationToken cancellationToken = default);

	/// <summary>Users whose <see cref="User.Roles"/> include <paramref name="role"/> (for notification fan-out).</summary>
	Task<IReadOnlyList<Guid>> GetIdsHavingRoleAsync(UserRole role, CancellationToken cancellationToken = default);
	Task AddAsync(User user, CancellationToken cancellationToken = default);
	Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
	Task UpdateUserProfile(Guid userId, string? newName, string? newEmail);

}
