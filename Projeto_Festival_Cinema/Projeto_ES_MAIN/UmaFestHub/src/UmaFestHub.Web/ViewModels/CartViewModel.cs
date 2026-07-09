namespace UmaFestHub.Web.ViewModels;

public sealed class CartViewModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public IReadOnlyList<CartItemViewModel> Items { get; set; } = [];
	public decimal TotalAmount => Items.Sum(x => x.Price * x.Quantity);
}

public sealed class CartItemViewModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string ProductType { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal Price { get; set; }
}

