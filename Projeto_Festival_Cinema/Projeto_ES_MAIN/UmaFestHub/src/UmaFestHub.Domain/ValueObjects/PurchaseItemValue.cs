namespace UmaFestHub.Domain.ValueObjects;

public record PurchaseItemValue
{
	public Guid ProductId { get; init; }
	public decimal PriceAtPurchase { get; init; }
}
