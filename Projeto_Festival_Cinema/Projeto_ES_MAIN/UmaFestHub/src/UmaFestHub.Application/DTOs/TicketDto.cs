namespace UmaFestHub.Application.DTOs;

public sealed record TicketDto(
    Guid Id,
    Guid SessionId,
    string ProductType,
    decimal Price,
    string TicketNumber);
