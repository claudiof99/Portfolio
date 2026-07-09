using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Validators.Purchase;

public sealed class FestivalActiveRule : IPurchaseValidator
{
	private readonly IFestivalService _festivalService;

	public string ProductType => "*";

	public FestivalActiveRule(IFestivalService festivalService)
	{
		_festivalService = festivalService;
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		var festivalId = product.GetFestivalId();
		if (!festivalId.HasValue)
		{
			return PurchaseValidationResult.Success();
		}

		var festival = await _festivalService.GetByIdAsync(festivalId.Value, cancellationToken);
		if (festival is null)
		{
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_FestivalNotFound));
		}

		if (DateTime.UtcNow > festival.EndDateUtc)
		{
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_FestivalEnded, festival.Name));
		}

		return PurchaseValidationResult.Success();
	}
}
