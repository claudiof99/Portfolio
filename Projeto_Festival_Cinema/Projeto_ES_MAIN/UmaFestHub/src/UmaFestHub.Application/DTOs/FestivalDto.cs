namespace UmaFestHub.Application.DTOs;

public sealed record FestivalDto(
	Guid Id,
	string Name,
	string Description,
	DateTime StartDateUtc,
	DateTime EndDateUtc,
	decimal? EarlyBirdDiscountPercent,
	int? EarlyBirdDaysBeforeStart,
	bool IsHidden = false);
