using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Factories;

public sealed class ProductFactory : IProductFactory
{
	private readonly Dictionary<string, ProductStore> _stores;
	private readonly Dictionary<string, string> _passTypeAliases;

	public ProductFactory()
	{
		_stores = new Dictionary<string, ProductStore>(StringComparer.OrdinalIgnoreCase)
		{
			[nameof(Ticket)] = new TicketStore(),
			[nameof(DailyPass)] = new DailyPassStore(),
			[nameof(CompletePass)] = new CompletePassStore(),
			[nameof(Rental)] = new RentalStore()
		};

		// Aliases for UI pass types
		_passTypeAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["Day"] = "DailyPass",
			["DailyPass"] = "DailyPass",
			["Full"] = "CompletePass",
			["Complete"] = "CompletePass",
			["CompletePass"] = "CompletePass"
		};
	}

	public ProductStore? GetStore(string productType)
	{
		var normalizedType = NormalizePassType(productType);
		return _stores.TryGetValue(normalizedType, out var store) ? store : null;
	}

	public IReadOnlyList<ProductStore> GetAllStores() => _stores.Values.ToList();

	public Product Create(
		string productType,
		decimal price,
		Guid? sessionId = null,
		Guid? festivalId = null,
		Guid? festivalFilmId = null,
		DateTime? dateUtc = null,
		Duration? duration = null)
	{
		var normalizedType = NormalizePassType(productType);
		var store = GetStore(normalizedType)
			?? throw new ArgumentException($"Unknown product type: {productType}. Available types: {string.Join(", ", _stores.Keys)}");

		return store.Create(price, sessionId, festivalId, festivalFilmId, dateUtc, duration);
	}

	private string NormalizePassType(string passType)
	{
		if (_passTypeAliases.TryGetValue(passType, out var normalized))
		{
			return normalized;
		}
		return passType;
	}

	public string GetNormalizedPassType(string passType) => NormalizePassType(passType);
}