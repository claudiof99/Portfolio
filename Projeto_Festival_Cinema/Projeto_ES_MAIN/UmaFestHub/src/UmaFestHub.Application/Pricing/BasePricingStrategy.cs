using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Pricing;

public sealed class BasePricingStrategy : IPricingStrategy
{
    public decimal Calculate(Product product, PricingContext context)
        => product.Price;
}
