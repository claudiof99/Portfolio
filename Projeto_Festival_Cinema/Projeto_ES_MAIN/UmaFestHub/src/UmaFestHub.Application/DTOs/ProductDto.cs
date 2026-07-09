namespace UmaFestHub.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string ProductType,
    decimal Price);
