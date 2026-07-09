using UmaFestHub.Domain.ValueObjects;
namespace UmaFestHub.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class Film
{
	public Guid Id { get; set; }
	public int ExternalId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Url { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
	public string Description { get; set; } = string.Empty;
	public Duration Duration { get; set; } = new() { Value = 120, Unit = DurationUnit.Minutes };
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public decimal TmdbPopularity { get; set; }

	[NotMapped]
	public string Title
	{
		get => Name;
		set => Name = value;
	}

	public ICollection<Genre> Genres { get; set; } = new List<Genre>();
	public ICollection<CreditFilm> Credits { get; set; } = new List<CreditFilm>();
	public ICollection<FestivalFilm> FestivalFilms { get; set; } = new List<FestivalFilm>();
}
