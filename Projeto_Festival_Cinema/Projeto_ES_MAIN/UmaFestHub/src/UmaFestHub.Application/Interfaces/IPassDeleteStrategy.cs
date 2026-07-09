using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Interfaces;

public interface IPassDeleteStrategy
{
	string PassType { get; }
	Task CreateOrUpdateAsync(IProductRepository repository, Guid festivalId, decimal newPrice, CancellationToken cancellationToken = default);
}

public sealed class DailyPassDeleteStrategy : IPassDeleteStrategy
{
	public string PassType => "DailyPass";

	public async Task CreateOrUpdateAsync(IProductRepository repository, Guid festivalId, decimal newPrice, CancellationToken cancellationToken = default)
	{
		var existing = await repository.GetDailyPassAsync(festivalId, cancellationToken);
		if (existing != null)
		{
			existing.Price = newPrice;
			await repository.UpdateAsync(existing, cancellationToken);
		}
		else
		{
			var newPass = new DailyPass(festivalId, newPrice, DateTime.UtcNow)
			{
				Id = Guid.NewGuid()
			};
			await repository.AddAsync(newPass, cancellationToken);
		}
	}
}

public sealed class CompletePassDeleteStrategy : IPassDeleteStrategy
{
	public string PassType => "CompletePass";

	public async Task CreateOrUpdateAsync(IProductRepository repository, Guid festivalId, decimal newPrice, CancellationToken cancellationToken = default)
	{
		var existing = await repository.GetCompletePassAsync(festivalId, cancellationToken);
		if (existing != null)
		{
			existing.Price = newPrice;
			await repository.UpdateAsync(existing, cancellationToken);
		}
		else
		{
			var newPass = new CompletePass(festivalId, newPrice)
			{
				Id = Guid.NewGuid()
			};
			await repository.AddAsync(newPass, cancellationToken);
		}
	}
}