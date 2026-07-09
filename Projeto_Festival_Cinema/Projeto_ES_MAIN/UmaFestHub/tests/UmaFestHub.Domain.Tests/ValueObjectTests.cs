using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using Xunit;

namespace UmaFestHub.Domain.Tests;

public class ValueObjectTests
{
	[Fact]
	public void Duration_ToMinutes_ShouldConvertHours()
	{
		var duration = new Duration { Value = 2, Unit = DurationUnit.Hours };
		Assert.Equal(120, duration.ToMinutes());
	}

	[Fact]
	public void Purchase_MarkCompleted_ShouldSetStatus()
	{
		var purchase = new Purchase();
		purchase.MarkCompleted();
		Assert.Equal(PurchaseStatus.Completed, purchase.Status);
	}
}
