namespace UmaFestHub.Web.ViewModels;

public sealed class PurchaseViewModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public DateTime DateUtc { get; set; }
	public decimal TotalAmount { get; set; }
	public string Status { get; set; } = string.Empty;
	public TimeSpan? ActiveRentalRemaining { get; set; }
	public IReadOnlyList<PurchaseItemViewModel> Items { get; set; } = [];
}

public sealed class PurchaseItemViewModel
{
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal PriceAtPurchase { get; set; }
}

