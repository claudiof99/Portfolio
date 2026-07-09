using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
	private readonly AppDbContext _dbContext;

	public CartRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
		=> await _dbContext.Carts
			.Include(x => x.CartItems)
				.ThenInclude(x => x.Product)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == CartStatus.Active, cancellationToken);

	public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
	{
		await _dbContext.Carts.AddAsync(cart, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
	{
		await _dbContext.CartItems.AddAsync(cartItem,cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task RemoveItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
	{
		_dbContext.CartItems.Remove(cartItem);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
	{
		_dbContext.Carts.Update(cart);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task ClearCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
	{
		var items = await _dbContext.Set<CartItem>()
			.Where(x => x.CartId == cartId)
			.ToListAsync(cancellationToken);

		_dbContext.Set<CartItem>().RemoveRange(items);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
