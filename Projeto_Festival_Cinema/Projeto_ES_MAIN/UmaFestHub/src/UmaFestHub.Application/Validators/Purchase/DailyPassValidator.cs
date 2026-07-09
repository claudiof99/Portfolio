using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Validators.Purchase;

/// <summary>
/// Rule: User cannot already own a DailyPass for this festival.
/// Prevents duplicate daily pass purchases.
/// </summary>
public sealed class DailyPassValidator : IPurchaseValidator
{
	private readonly IPurchaseRepository _purchaseRepository;

	public string ProductType => nameof(DailyPass);

	public DailyPassValidator(IPurchaseRepository purchaseRepository)
	{
		_purchaseRepository = purchaseRepository;
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		if (product is not DailyPass dailyPass)
			return PurchaseValidationResult.Success();

		var purchases = await _purchaseRepository.GetByUserIdAsync(userId, cancellationToken);
		var hasExisting = purchases
			.Where(p => p.Status == Domain.Entities.PurchaseStatus.Completed)
			.Any(p => p.PurchaseItems.Any(i => i.Product is DailyPass existing && existing.FestivalId == dailyPass.FestivalId));

		if (hasExisting)
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_AlreadyOwnDailyPass));

		return PurchaseValidationResult.Success();
	}
}