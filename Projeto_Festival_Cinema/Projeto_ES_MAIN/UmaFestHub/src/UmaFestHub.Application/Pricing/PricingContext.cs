namespace UmaFestHub.Application.Pricing;

public sealed class PricingContext
{
    public required Domain.Entities.User User { get; init; }
    public required DateTime PurchaseDate { get; init; }
    public Domain.Entities.Festival? Festival { get; init; }
}
