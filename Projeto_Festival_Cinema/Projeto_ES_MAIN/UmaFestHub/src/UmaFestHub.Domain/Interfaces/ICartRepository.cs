using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface ICartRepository
{
	Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
	Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
	Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default);
	Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
	Task RemoveItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
	Task ClearCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default);
}
