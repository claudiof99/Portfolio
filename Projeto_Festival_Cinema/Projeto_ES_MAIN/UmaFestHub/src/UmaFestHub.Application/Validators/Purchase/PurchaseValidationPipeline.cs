using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Validators.Purchase;

public sealed class PurchaseValidationPipeline
{
	private readonly IEnumerable<IPurchaseValidator> _validators;
	private readonly IPurchaseValidator? _festivalRule;

	public PurchaseValidationPipeline(IEnumerable<IPurchaseValidator> validators)
	{
		_validators = validators.Where(v => v.ProductType != "*").ToList();
		_festivalRule = validators.FirstOrDefault(v => v.ProductType == "*");
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		if (_festivalRule != null)
		{
			var festivalResult = await _festivalRule.ValidateAsync(userId, product, cancellationToken);
			if (!festivalResult.IsValid)
			{
				return festivalResult;
			}
		}

		var applicableValidator = _validators.FirstOrDefault(v =>
			v.ProductType.Equals(product.ProductType, StringComparison.OrdinalIgnoreCase));

		if (applicableValidator != null)
		{
			return await applicableValidator.ValidateAsync(userId, product, cancellationToken);
		}

		return PurchaseValidationResult.Success();
	}

	public async Task<PurchaseValidationResult> ValidateCartAsync(
		Guid userId,
		IReadOnlyList<CartItemValidationInput> items,
		CancellationToken cancellationToken = default)
	{
		var errors = new List<UserMessage>();

		foreach (var item in items)
		{
			var result = await ValidateAsync(userId, item.Product, cancellationToken);
			if (!result.IsValid)
			{
				errors.AddRange(result.Errors);
			}
		}

		return errors.Count > 0
			? PurchaseValidationResult.Failure(errors)
			: PurchaseValidationResult.Success();
	}
}

public record CartItemValidationInput(Product Product);
