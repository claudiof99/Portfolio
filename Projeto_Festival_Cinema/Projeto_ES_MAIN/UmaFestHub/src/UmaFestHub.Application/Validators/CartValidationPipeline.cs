using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Validators;

public interface ICartValidationPipeline
{
	Task<CartValidationResult> ValidateAsync(Guid userId, IReadOnlyList<CartItemDto> items, CancellationToken cancellationToken = default);
}

public sealed class CartValidationPipeline : ICartValidationPipeline
{
	private readonly IEnumerable<ICartValidator> _validators;

	public CartValidationPipeline(IEnumerable<ICartValidator> validators)
	{
		_validators = validators;
	}

	public async Task<CartValidationResult> ValidateAsync(Guid userId, IReadOnlyList<CartItemDto> items, CancellationToken cancellationToken = default)
	{
		var errors = new List<string>();

		foreach (var validator in _validators)
		{
			var result = await validator.ValidateAsync(userId, items, cancellationToken);
			if (!result.IsValid)
			{
				errors.AddRange(result.Errors);
			}
		}

		return errors.Count > 0
			? CartValidationResult.Failure(errors)
			: CartValidationResult.Success();
	}
}