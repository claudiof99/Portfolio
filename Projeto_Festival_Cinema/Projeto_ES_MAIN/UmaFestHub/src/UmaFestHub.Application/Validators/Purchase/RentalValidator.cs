using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Validators.Purchase;

/// <summary>
/// Rule: User cannot already own a Rental for this film.
/// Prevents duplicate rental purchases (rental is for 48h access).
/// </summary>
public sealed class RentalValidator : IPurchaseValidator
{
	private readonly IPurchaseRepository _purchaseRepository;

	public string ProductType => nameof(Rental);

	public RentalValidator(IPurchaseRepository purchaseRepository)
	{
		_purchaseRepository = purchaseRepository;
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		if (product is not Rental rental)
			return PurchaseValidationResult.Success();

		var purchases = await _purchaseRepository.GetByUserIdAsync(userId, cancellationToken);
		var hasExisting = purchases
			.Where(p => p.Status == Domain.Entities.PurchaseStatus.Completed)
			.Any(p => p.PurchaseItems.Any(i => i.Product is Rental existing && existing.FestivalFilmId == rental.FestivalFilmId));

		if (hasExisting)
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_AlreadyOwnRental));

		return PurchaseValidationResult.Success();
	}
}