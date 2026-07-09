using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Validators;

public sealed class DuplicateItemValidator : ICartValidator
{
	private readonly IProductService _productService;

	public string Name => nameof(DuplicateItemValidator);

	public DuplicateItemValidator(IProductService productService)
	{
		_productService = productService;
	}

	public async Task<CartValidationResult> ValidateAsync(Guid userId, IReadOnlyList<CartItemDto> items, CancellationToken cancellationToken = default)
	{
		var duplicateProductIds = items
			.GroupBy(x => x.ProductId)
			.Where(g => g.Count() > 1)
			.Select(g => g.Key)
			.ToList();

		if (duplicateProductIds.Count == 0)
		{
			return CartValidationResult.Success();
		}

		var productNames = new List<string>();
		foreach (var productId in duplicateProductIds)
		{
			var product = await _productService.GetByIdAsync(productId, cancellationToken);
			if (product != null)
			{
				productNames.Add(product.ProductType);
			}
		}

		return CartValidationResult.Failure(
			$"Duplicate items found: {string.Join(", ", productNames)}. Please consolidate your cart items.");
	}
}