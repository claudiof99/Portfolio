using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using Xunit;

namespace UmaFestHub.Domain.Tests;

public class UserTests
{
	[Fact]
	public void NewUser_HasCustomerRoleByDefault()
	{
	var user = new User();

		Assert.Contains(UserRole.Customer, user.Roles);
	}
}
