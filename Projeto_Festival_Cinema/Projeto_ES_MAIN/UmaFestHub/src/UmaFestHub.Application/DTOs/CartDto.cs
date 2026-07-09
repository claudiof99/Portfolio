namespace UmaFestHub.Application.DTOs;

public sealed record CartDto(
	Guid Id,
	Guid UserId,
	IReadOnlyList<CartItemDto> Items);

public sealed record CartItemDto(
	Guid Id,
	Guid ProductId,
	string ProductType,
	int Quantity,
	decimal Price);
