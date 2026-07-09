namespace UmaFestHub.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using UmaFestHub.Domain.Enums;

public class Purchase
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public DateTime DateUtc { get; set; } = DateTime.UtcNow;
	public decimal TotalAmount { get; set; }
	public PurchaseStatus Status { get; private set; } = PurchaseStatus.Pending;

	[NotMapped]
	public DateTime PurchasedAtUtc
	{
		get => DateUtc;
		set => DateUtc = value;
	}

	public User? User { get; set; }
	public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

	public void MarkCompleted()
	{
		if (Status is PurchaseStatus.Completed or PurchaseStatus.Cancelled)
		{
			throw new InvalidOperationException($"Cannot complete purchase in '{Status}' status.");
		}

		Status = PurchaseStatus.Completed;
	}

	public void MarkFailed()
	{
		if (Status is PurchaseStatus.Completed or PurchaseStatus.Cancelled)
		{
			throw new InvalidOperationException($"Cannot fail purchase in '{Status}' status.");
		}

		Status = PurchaseStatus.Failed;
	}
}
