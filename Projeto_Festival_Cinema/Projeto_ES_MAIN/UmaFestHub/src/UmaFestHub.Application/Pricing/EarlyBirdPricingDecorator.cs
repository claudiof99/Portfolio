using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Pricing;

public sealed class EarlyBirdPricingDecorator : IPricingStrategy
{
    private readonly IPricingStrategy _inner;

    public EarlyBirdPricingDecorator(IPricingStrategy inner)
        => _inner = inner;

    public decimal Calculate(Product product, PricingContext context)
    {
        var price = _inner.Calculate(product, context);

        // Only apply early bird discount to passes (DailyPass and CompletePass)
        if (product is not (DailyPass or CompletePass))
            return price;

        if (context.Festival is null)
            return price;

        var discountPercent = context.Festival.EarlyBirdDiscountPercent;
        var daysBeforeStart = context.Festival.EarlyBirdDaysBeforeStart;

        if (!discountPercent.HasValue || !daysBeforeStart.HasValue)
            return price;

        // Check if purchase is early enough (before the early bird cutoff date)
        var earlyBirdCutoffDate = context.Festival.StartDateUtc.AddDays(-daysBeforeStart.Value);
        if (context.PurchaseDate < earlyBirdCutoffDate)
        {
            var discountMultiplier = 1m - (discountPercent.Value / 100m);
            return Math.Round(price * discountMultiplier, 2);
        }

        return price;
    }
}
