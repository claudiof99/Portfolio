namespace UmaFestHub.Application.Pricing;

public interface IPricingService
{
    Task<decimal> GetPriceAsync(Domain.Entities.Product product, Guid userId, DateTime purchaseDate, CancellationToken cancellationToken = default);
}
