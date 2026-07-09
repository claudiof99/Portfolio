namespace UmaFestHub.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class PurchaseItem
{
	public Guid Id { get; set; }
	public Guid PurchaseId { get; set; }
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal PriceAtPurchase { get; set; }

	[NotMapped]
	public decimal UnitPrice
	{
		get => PriceAtPurchase;
		set => PriceAtPurchase = value;
	}

	public Purchase? Purchase { get; set; }
	public Product? Product { get; set; }
}
