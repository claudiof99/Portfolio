using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Pricing;

public interface IPricingStrategy
{
    decimal Calculate(Product product, PricingContext context);
}
