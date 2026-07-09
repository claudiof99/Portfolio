namespace UmaFestHub.Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency = "USD")
{
	public static Money Zero(string currency = "USD") => new(0m, currency);
}
