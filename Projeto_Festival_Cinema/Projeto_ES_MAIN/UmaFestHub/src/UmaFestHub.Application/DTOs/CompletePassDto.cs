namespace UmaFestHub.Application.DTOs;


public sealed record CompletePassDto
(   
    Guid Id,
	decimal Price,
	string ProductType,
    Guid FestivalId
);