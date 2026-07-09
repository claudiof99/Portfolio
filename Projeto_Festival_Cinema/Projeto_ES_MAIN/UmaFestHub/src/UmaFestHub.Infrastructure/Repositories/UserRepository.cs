using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly AppDbContext _dbContext;

	public UserRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
		=> await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

	public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
		=> await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);

	public async Task<IReadOnlyList<Guid>> SearchIdsByNameAsync(string nameQuery, CancellationToken cancellationToken = default)
	{
		var query = (nameQuery ?? string.Empty).Trim();
		if (string.IsNullOrWhiteSpace(query))
		{
			return Array.Empty<Guid>();
		}

		return await _dbContext.Users
			.Where(x => x.Name.Contains(query))
			.Select(x => x.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Guid>> GetIdsHavingRoleAsync(UserRole role, CancellationToken cancellationToken = default)
	{
		// Roles are stored as a converted string; materialize and filter in memory for reliable matching.
		var rows = await _dbContext.Users
			.AsNoTracking()
			.Select(u => new { u.Id, u.Roles })
			.ToListAsync(cancellationToken);
		return rows.Where(r => r.Roles.Contains(role)).Select(r => r.Id).Distinct().ToList();
	}

	public async Task AddAsync(User user, CancellationToken cancellationToken = default)
	{
		await _dbContext.Users.AddAsync(user, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		_dbContext.Users.Update(user);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
	{
		_dbContext.Users.Remove(user);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateUserProfile(Guid userId, string? newName, string? newEmail)
	{
		var user = await _dbContext.Users.FindAsync(userId);
		if(user != null)
		{
			if (!string.IsNullOrWhiteSpace(newName))
				user.Name = newName;
			if (!string.IsNullOrWhiteSpace(newEmail))
				user.Email = newEmail;
			await _dbContext.SaveChangesAsync();
		}
	}
	
}
