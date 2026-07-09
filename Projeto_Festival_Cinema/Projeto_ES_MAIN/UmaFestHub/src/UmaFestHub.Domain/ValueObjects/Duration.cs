using System;
using UmaFestHub.Domain.Entities;
namespace UmaFestHub.Domain.ValueObjects;

//Duration value object to represent film durations 
//in a flexible way (e.g., 2 hours, 90 minutes, etc.). 
//This allows for easy conversion and display in different formats as needed.

public record Duration
{
	public int Value { get; init; }
	public DurationUnit Unit { get; init; } = DurationUnit.Minutes;

	public Duration()
	{
		
	}
	public Duration(int value, DurationUnit unit)
    {
        Value = value;
        Unit = unit;
    }


	public int ToMinutes() => Unit switch
	{
		DurationUnit.Minutes => Value,
		DurationUnit.Hours => Value * 60,
		DurationUnit.Days => Value * 24 * 60,
		_ => Value
	};

	public TimeSpan ToTimeSpan() => Unit switch
	{
		DurationUnit.Hours => TimeSpan.FromHours(Value),
		DurationUnit.Days  => TimeSpan.FromDays(Value),
		_                  => throw new InvalidOperationException($"Unknown DurationUnit: {Unit}")
	};
}
