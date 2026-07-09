using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Validators;

public sealed class ProductExistenceValidator : ICartValidator
{
	private readonly IProductService _productService;

	public string Name => nameof(ProductExistenceValidator);

	public ProductExistenceValidator(IProductService productService)
	{
		_productService = productService;
	}

	public async Task<CartValidationResult> ValidateAsync(Guid userId, IReadOnlyList<CartItemDto> items, CancellationToken cancellationToken = default)
	{
		var errors = new List<string>();

		foreach (var item in items)
		{
			var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);
			if (product == null)
			{
				errors.Add($"Product with ID {item.ProductId} no longer exists.");
			}
		}

		return errors.Count > 0
			? CartValidationResult.Failure(errors)
			: CartValidationResult.Success();
	}
}