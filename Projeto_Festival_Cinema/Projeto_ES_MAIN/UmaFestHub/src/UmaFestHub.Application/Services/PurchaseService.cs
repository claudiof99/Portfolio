using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Application.Observers.PurchaseCompleted;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using UmaFestHub.Application.Validators.Purchase;

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to handle the checkout process, integrate with payment simulation, and record purchase history.
/// After a successful payment the purchase is saved, then <see cref="IPurchaseCompletedNotifier"/> runs (in-app purchase-completed notification).
/// </summary>
public class PurchaseService : IPurchaseService
{
	private readonly IPurchaseRepository _purchaseRepository;
	private readonly IPaymentSimulationService _paymentSimulationService;
	private readonly ICartService _cartService;
	private readonly IProductRepository _productRepository;
	private readonly PurchaseValidationPipeline _validationPipeline;
	private readonly IPurchaseCompletedNotifier _purchaseNotifier;
	private readonly ILogger<PurchaseService> _logger;

	public PurchaseService(
		IPurchaseRepository purchaseRepository,
		IPaymentSimulationService paymentSimulationService,
		ICartService cartService,
		IProductRepository productRepository,
		PurchaseValidationPipeline validationPipeline,
		IPurchaseCompletedNotifier purchaseNotifier,
		ILogger<PurchaseService> logger)
	{
		_purchaseRepository = purchaseRepository;
		_paymentSimulationService = paymentSimulationService;
		_cartService = cartService;
		_productRepository = productRepository;
		_validationPipeline = validationPipeline;
		_purchaseNotifier = purchaseNotifier;
		_logger = logger;
	}

	/// <summary>
	/// We retrieve the complete purchase history for a specific user.
	/// </summary>
	public async Task<IReadOnlyList<PurchaseDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var purchases = await _purchaseRepository.GetByUserIdExcludingExpiredAsync(userId, cancellationToken);
		return purchases.Select(p => new PurchaseDto(
			p.Id,
			p.UserId,
			p.DateUtc,
			p.TotalAmount,
			p.Status.ToString(),
			null,
			p.PurchaseItems.Select(i => new PurchaseItemDto(i.ProductId, i.Quantity, i.PriceAtPurchase)).ToList())).ToList();
	}

	/// <summary>
	/// We process a checkout: validating products, calculating totals, simulating payment, and recording the transaction.
	/// </summary>
	public async Task<Guid> CheckoutAsync(Guid userId, IReadOnlyList<PurchaseItemDto> items, CancellationToken cancellationToken = default)
	{
		if (items.Count == 0)
		{
			throw new UserFacingException(new UserMessage(UserMessageKeys.Purchase_CheckoutEmpty));
		}

		// Validate all items before processing
		var productIds = items.Select(i => i.ProductId).ToHashSet();
		var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

		var validationItems = products.Select(p => new CartItemValidationInput(p)).ToList();
		var validationResult = await _validationPipeline.ValidateCartAsync(userId, validationItems, cancellationToken);

		if (!validationResult.IsValid)
		{
			var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.Key));
			_logger.LogWarning("Checkout validation failed for user {UserId}: {Errors}", userId, errorMessage);
			throw new UserFacingException(validationResult.Errors);
		}

		var purchase = new Purchase
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			DateUtc = DateTime.UtcNow,
			TotalAmount = items.Sum(x => x.PriceAtPurchase * x.Quantity),
			PurchaseItems = items.Select(i => new PurchaseItem
			{
				Id = Guid.NewGuid(),
				ProductId = i.ProductId,
				Quantity = i.Quantity,
				PriceAtPurchase = i.PriceAtPurchase
			}).ToList()
		};

		string? transactionId = null;

		// Only attempt payment processing if the total amount is greater than zero
		if (purchase.TotalAmount > 0)
		{
			var payment = await _paymentSimulationService.ProcessAsync(userId, purchase.TotalAmount, cancellationToken);
			if (!payment.IsSuccessful)
			{
				purchase.MarkFailed();
				_logger.LogWarning("Checkout payment failed for user {UserId}: {Error}", userId, payment.ErrorMessage);
				throw new UserFacingException(new UserMessage(UserMessageKeys.Purchase_PaymentFailed));
			}
			transactionId = payment.TransactionId;
		}

		purchase.MarkCompleted();
		await _purchaseRepository.AddAsync(purchase, cancellationToken);

		// Observer fires after purchase is persisted — service layer, not controller
		await _purchaseNotifier.NotifyAsync(new PurchaseCompletedContext
		{
			UserId = userId,
			PurchaseId = purchase.Id,
			TotalAmount = purchase.TotalAmount,
			CompletedAt = purchase.DateUtc
		}, cancellationToken);

		await _cartService.CheckOutCartAsync(userId, cancellationToken);

		_logger.LogInformation("Checkout completed for user {UserId} with transaction {TransactionId}", userId, transactionId ?? "FREE-CHECKOUT");
		return purchase.Id;
	}
}