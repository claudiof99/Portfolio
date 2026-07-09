namespace UmaFestHub.Application.DTOs;

public sealed record AdminDashboardStatsDto(
	int TotalFestivals,
	int TotalFilms,
	int ActiveSessions,
	int TotalPurchases,
	bool IsServiceHealthy);
