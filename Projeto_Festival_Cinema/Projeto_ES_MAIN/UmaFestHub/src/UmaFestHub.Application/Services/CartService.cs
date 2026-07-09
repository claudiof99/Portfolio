using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Application.Validators;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to manage the shopping cart, allowing users to add, view, and remove items before checkout.
/// </summary>
public class CartService : ICartService
{
	private readonly ICartRepository _cartRepository;
	private readonly ICartValidationPipeline _validationPipeline;
	private readonly IProductRepository _productRepository;

	public CartService(
		ICartRepository cartRepository,
		ICartValidationPipeline validationPipeline,
		IProductRepository productRepository)
	{
		_cartRepository = cartRepository;
		_validationPipeline = validationPipeline;
		_productRepository = productRepository;
	}

	/// <summary>
	/// We fetch the current active cart for a specific user.
	/// </summary>
	public async Task<CartDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null)
		{
			return null;
		}

		return new CartDto(
			cart.Id,
			cart.UserId,
			cart.CartItems.Select(item => new CartItemDto(
				item.Id,
				item.ProductId,
				item.Product?.ProductType ?? "Unknown",
				item.Quantity,
				item.Product?.Price ?? 0m)).ToList());
	}

	/// <summary>
	/// We remove a specific product from the user's cart.
	/// </summary>
	public async Task RemoveItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
	{
		var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null)
		{
			return;
		}

		var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
		if (item != null)
		{
			await _cartRepository.RemoveItemAsync(item, cancellationToken);
			await _cartRepository.UpdateAsync(cart, cancellationToken);
		}
	}

	/// <summary>
	/// We add a product to the user's cart. For passes and rentals, duplicates are not allowed.
	/// Tickets can be added multiple times.
	/// </summary>
	public async Task AddProductAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default)
	{
		if (quantity <= 0)
		{
			throw new UserFacingException(new UserMessage(UserMessageKeys.Cart_QuantityInvalid));
		}

		var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
		if (product == null)
		{
			throw new UserFacingException(new UserMessage(UserMessageKeys.Cart_ProductNotFound));
		}

		var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null)
		{
			cart = new Cart
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				CartItems = new List<CartItem>()
			};

			await _cartRepository.AddAsync(cart, cancellationToken);
		}

		var existing = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

		// For passes and rentals, prevent duplicates
		if (existing != null && (product is DailyPass || product is CompletePass || product is Rental))
		{
			throw new UserFacingException(new UserMessage(UserMessageKeys.Cart_DuplicateItem, product.ProductType));
		}

		// For tickets, allow quantity increase
		if (existing is null)
		{
			var cartItem = new CartItem
			{
				Id = Guid.NewGuid(),
				CartId = cart.Id,
				ProductId = productId,
				Quantity = quantity
			};

			await _cartRepository.AddItemAsync(cartItem, cancellationToken);
		}
		else
		{
			// Only for tickets - increment quantity
			existing.Quantity += quantity;
		}

		await _cartRepository.UpdateAsync(cart, cancellationToken);
	}

	/// <summary>
	/// We transition the user's active cart to CheckedOut after a successful purchase.
	/// Items are preserved for order history.
	/// </summary>
	public async Task CheckOutCartAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null) return;

		cart.CheckOut();
		await _cartRepository.UpdateAsync(cart, cancellationToken);
	}

	/// <summary>
	/// Validate cart items using the validation pipeline (Chain of Responsibility).
	/// </summary>
	public async Task<CartValidationResult> ValidateCartAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
		if (cart is null || cart.CartItems.Count == 0)
		{
			return CartValidationResult.Success();
		}

		var items = cart.CartItems.Select(item => new CartItemDto(
			item.Id,
			item.ProductId,
			item.Product?.ProductType ?? "Unknown",
			item.Quantity,
			item.Product?.Price ?? 0m)).ToList();

		return await _validationPipeline.ValidateAsync(userId, items, cancellationToken);
	}
}