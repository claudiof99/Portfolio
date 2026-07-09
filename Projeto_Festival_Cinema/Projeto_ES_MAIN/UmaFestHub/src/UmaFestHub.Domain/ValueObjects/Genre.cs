using UmaFestHub.Domain.Entities;
namespace UmaFestHub.Domain.ValueObjects;

public record Genre
{
	public Guid Id { get; init; }
	public Guid FilmId { get; init; }
	public string Name { get; init; } = string.Empty;
	public Film? Film { get; init; }
}
