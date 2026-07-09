
using System;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Strategies;

public class CompletePassEntitlementStrategy : IEntitlementStrategy
{
    private readonly IProductService _productService;
    private readonly IFestivalService _festivalService;

    public Type ProductDomainType => typeof(CompletePass);

    public CompletePassEntitlementStrategy(IProductService productService, IFestivalService festivalService)
    {
        _productService = productService;
        _festivalService = festivalService;
    }

    public string ProductType => "CompletePass";

    public async Task<bool> GrantsAccessAsync(Guid productId, DateTime purchaseDateUtc, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default)
    {
        var pass = await _productService.GetCompletePassDtoAsync(festivalId, cancellationToken);
        if (pass == null || pass.Id != productId)
        {
            return false;
        }

        var festival = await _festivalService.GetByIdAsync(festivalId, cancellationToken);
        if (festival == null)
        {
            return false;
        }

        return DateTime.UtcNow >= festival.StartDateUtc && DateTime.UtcNow <= festival.EndDateUtc;
    }
}