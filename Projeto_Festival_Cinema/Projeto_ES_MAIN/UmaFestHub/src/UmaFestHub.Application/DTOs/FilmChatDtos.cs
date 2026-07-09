namespace UmaFestHub.Application.DTOs;

public sealed class FilmSearchIntent
{
	public string? Title { get; set; }
	public string? Genre { get; set; }
	public int? MinDurationMinutes { get; set; }
	public int? MaxDurationMinutes { get; set; }
	public int? ReleaseYear { get; set; }

	public bool HasAnyFilter =>
		!string.IsNullOrWhiteSpace(Title)
		|| !string.IsNullOrWhiteSpace(Genre)
		|| MinDurationMinutes.HasValue
		|| MaxDurationMinutes.HasValue
		|| ReleaseYear.HasValue;
}

public sealed record FilmChatMatchDto(
	Guid Id,
	string Name,
	IReadOnlyList<string> Genres,
	int DurationMinutes,
	int? ReleaseYear,
	string? ImageUrl);

public sealed record FilmChatResultDto(
	string Reply,
	IReadOnlyList<FilmChatMatchDto> Matches,
	FilmSearchIntent AppliedFilters,
	bool UsedAi);
