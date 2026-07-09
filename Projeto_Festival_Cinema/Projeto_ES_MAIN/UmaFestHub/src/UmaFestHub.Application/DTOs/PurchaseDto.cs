namespace UmaFestHub.Application.DTOs;

public sealed record PurchaseDto(
	Guid Id,
	Guid UserId,
	DateTime DateUtc,
	decimal TotalAmount,
	string Status,
	TimeSpan? ActiveRentalRemaining,
	IReadOnlyList<PurchaseItemDto> Items);

public sealed record PurchaseItemDto(
	Guid ProductId,
	int Quantity,
	decimal PriceAtPurchase);
