using UmaFestHub.Domain.Entities;
using Xunit;

namespace UmaFestHub.Domain.Tests;

public class PurchaseTests
{
	[Fact]
	public void Ticket_ShouldBehaveAsProduct()
	{
		Product ticket = new Ticket();

		Assert.Equal("Ticket", ticket.ProductType);
	}
}
