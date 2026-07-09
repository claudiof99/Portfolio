namespace UmaFestHub.Application.DTOs;

// Simple paging wrapper used by services to return a page without requiring COUNT(*) queries.
// HasNext is computed by fetching one extra row in the repository/service layer.
public sealed record PagedResultDto<T>(
	IReadOnlyList<T> Items,
	int Page,
	bool HasNext);

