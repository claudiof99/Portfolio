using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Validators;

namespace UmaFestHub.Application.Interfaces;

public interface ICartService
{
	Task<CartDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
	Task AddProductAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default);
	Task RemoveItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
	Task CheckOutCartAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<CartValidationResult> ValidateCartAsync(Guid userId, CancellationToken cancellationToken = default);
}
