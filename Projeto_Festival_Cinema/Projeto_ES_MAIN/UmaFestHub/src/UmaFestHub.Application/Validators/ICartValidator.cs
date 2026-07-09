using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Validators;

public interface ICartValidator
{
	string Name { get; }
	Task<CartValidationResult> ValidateAsync(Guid userId, IReadOnlyList<CartItemDto> items, CancellationToken cancellationToken = default);
}