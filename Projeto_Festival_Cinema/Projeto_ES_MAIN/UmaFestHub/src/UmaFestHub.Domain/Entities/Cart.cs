using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Domain.Entities;

public class Cart
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public CartStatus Status { get; private set; } = CartStatus.Active;

	public User? User { get; set; }
	public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

	public void CheckOut()
	{
		if (Status != CartStatus.Active)
			throw new InvalidOperationException("Only an active cart can be checked out.");
		Status = CartStatus.CheckedOut;
	}

	public void Abandon()
	{
		if (Status != CartStatus.Active)
			throw new InvalidOperationException("Only an active cart can be abandoned.");
		Status = CartStatus.Abandoned;
	}
}
