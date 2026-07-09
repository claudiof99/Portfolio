using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Validators.Purchase;

/// <summary>
/// Rule: User cannot already own a CompletePass for this festival.
/// Prevents duplicate complete pass purchases.
/// </summary>
public sealed class CompletePassValidator : IPurchaseValidator
{
	private readonly IPurchaseRepository _purchaseRepository;

	public string ProductType => nameof(CompletePass);

	public CompletePassValidator(IPurchaseRepository purchaseRepository)
	{
		_purchaseRepository = purchaseRepository;
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		if (product is not CompletePass completePass)
			return PurchaseValidationResult.Success();

		var purchases = await _purchaseRepository.GetByUserIdAsync(userId, cancellationToken);
		var hasExisting = purchases
			.Where(p => p.Status == Domain.Entities.PurchaseStatus.Completed)
			.Any(p => p.PurchaseItems.Any(i => i.Product is CompletePass existing && existing.FestivalId == completePass.FestivalId));

		if (hasExisting)
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_AlreadyOwnCompletePass));

		return PurchaseValidationResult.Success();
	}
}