using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Validators.Purchase;

public interface IPurchaseValidator
{
	string ProductType { get; }
	Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default);
}