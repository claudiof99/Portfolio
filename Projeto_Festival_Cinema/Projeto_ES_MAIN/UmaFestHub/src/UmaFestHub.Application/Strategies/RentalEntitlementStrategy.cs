using System;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Strategies;

public class RentalEntitlementStrategy : IEntitlementStrategy
{
    private readonly IProductService _productService;

    public Type ProductDomainType => typeof(Rental);

    public RentalEntitlementStrategy(IProductService productService)
    {
        _productService = productService;
    }

    public string ProductType => "Rental";

    public async Task<bool> GrantsAccessAsync(Guid productId, DateTime purchaseDateUtc, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default)
    {
        var rental = await _productService.GetRentalDtoAsync(festivalFilmId, cancellationToken);
        if (rental == null || rental.Id != productId) return false;

        var durationUnit = rental.DurationUnit.ToLowerInvariant();
        DateTime expiryDate = purchaseDateUtc;

        if (durationUnit.Contains("hour"))
        {
            expiryDate = purchaseDateUtc.AddHours(rental.DurationValue);
        }
        else if (durationUnit.Contains("day"))
        {
            expiryDate = purchaseDateUtc.AddDays(rental.DurationValue);
        }
        else if (durationUnit.Contains("minute"))
        {
            expiryDate = purchaseDateUtc.AddMinutes(rental.DurationValue);
        }

        return DateTime.UtcNow <= expiryDate;
    }
}