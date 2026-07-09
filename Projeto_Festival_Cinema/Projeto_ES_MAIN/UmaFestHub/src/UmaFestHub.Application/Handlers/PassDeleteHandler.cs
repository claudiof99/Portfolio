using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Handlers;

public interface IPassDeleteHandler
{
	Task CreateOrUpdateAsync(string passType, Guid festivalId, decimal newPrice, CancellationToken cancellationToken = default);
}

public sealed class PassDeleteHandler : IPassDeleteHandler
{
	private readonly Dictionary<string, IPassDeleteStrategy> _strategies;
	private readonly IProductRepository _productRepository;
	private readonly Dictionary<string, string> _passTypeAliases;

	public PassDeleteHandler(IEnumerable<IPassDeleteStrategy> strategies, IProductRepository productRepository)
	{
		_strategies = strategies.ToDictionary(s => s.PassType, StringComparer.OrdinalIgnoreCase);
		_productRepository = productRepository;

		_passTypeAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["Day"] = "DailyPass",
			["DailyPass"] = "DailyPass",
			["Full"] = "CompletePass",
			["Complete"] = "CompletePass",
			["CompletePass"] = "CompletePass"
		};
	}

	public async Task CreateOrUpdateAsync(string passType, Guid festivalId, decimal newPrice, CancellationToken cancellationToken = default)
	{
		var normalizedType = NormalizePassType(passType);
		if (_strategies.TryGetValue(normalizedType, out var strategy))
		{
			await strategy.CreateOrUpdateAsync(_productRepository, festivalId, newPrice, cancellationToken);
		}
	}

	private string NormalizePassType(string passType)
	{
		if (_passTypeAliases.TryGetValue(passType, out var normalized))
		{
			return normalized;
		}
		return passType;
	}
}