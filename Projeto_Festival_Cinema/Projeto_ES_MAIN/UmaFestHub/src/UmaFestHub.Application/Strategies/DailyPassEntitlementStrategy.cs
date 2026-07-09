using System;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Strategies;

public class DailyPassEntitlementStrategy : IEntitlementStrategy
{
    private readonly IProductService _productService;

    public Type ProductDomainType => typeof(DailyPass);

    public DailyPassEntitlementStrategy(IProductService productService)
    {
        _productService = productService;
    }

    public string ProductType => "DailyPass";

    public async Task<bool> GrantsAccessAsync(Guid productId, DateTime purchaseDateUtc, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default)
    {
        var pass = await _productService.GetDailyPassDtoAsync(festivalId, cancellationToken);
        if (pass == null || pass.Id != productId) return false;

        var expiresAtUtc = purchaseDateUtc.AddDays(1);
        return DateTime.UtcNow >= purchaseDateUtc && DateTime.UtcNow <= expiresAtUtc;
    }
}