namespace UmaFestHub.Domain.ValueObjects;

public readonly record struct DateRange(DateTime StartUtc, DateTime EndUtc)
{
	public bool IsValid => EndUtc >= StartUtc;

	public bool Contains(DateTime pointUtc) => pointUtc >= StartUtc && pointUtc <= EndUtc;
}
