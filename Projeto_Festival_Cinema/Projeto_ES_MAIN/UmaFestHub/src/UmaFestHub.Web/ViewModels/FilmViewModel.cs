using UmaFestHub.Application.DTOs;
namespace UmaFestHub.Web.ViewModels;

public sealed class FilmViewModel
{
	public Guid Id { get; set; }
	public int ExternalId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Url { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
	public string Description { get; set; } = string.Empty;
	public int DurationMinutes { get; set; }
	public IReadOnlyList<string> Genres { get; set; } = [];
	public IReadOnlyList<FilmCreditDto> Credits { get; set; } = [];
}
