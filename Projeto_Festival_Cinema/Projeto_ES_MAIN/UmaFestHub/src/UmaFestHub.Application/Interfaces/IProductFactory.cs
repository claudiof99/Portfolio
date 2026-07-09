using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Application.Factories;

namespace UmaFestHub.Application.Interfaces;

public interface IProductFactory
{
	ProductStore? GetStore(string productType);
	IReadOnlyList<ProductStore> GetAllStores();
	Product Create(string productType, decimal price, Guid? sessionId = null, Guid? festivalId = null, Guid? festivalFilmId = null, DateTime? dateUtc = null, Duration? duration = null);
	string GetNormalizedPassType(string passType);
}