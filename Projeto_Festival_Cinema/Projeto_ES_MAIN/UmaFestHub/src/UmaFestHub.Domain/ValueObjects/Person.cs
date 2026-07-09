namespace UmaFestHub.Domain.ValueObjects;

public record Person
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
}
