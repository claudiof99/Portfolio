using UmaFestHub.Domain.Entities;
namespace UmaFestHub.Domain.ValueObjects;

public record CreditFilm
{
	public Guid Id { get; init; }
	public Guid FilmId { get; init; }
	public string Role { get; init; } = string.Empty;
	public Guid PersonId { get; init; }
	public Film? Film { get; init; }
	public Person? Person { get; init; }
}
