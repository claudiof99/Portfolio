namespace UmaFestHub.Application.DTOs;

public sealed record DailyPassDto
(
    Guid Id,
	decimal Price,
	string ProductType,
    Guid FestivalId,
    DateTime DateUtc
);