using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

public class EntitlementService : IEntitlementService
{
    private readonly IPurchaseService _purchaseService;
    private readonly IProductRepository _productRepository;
    private readonly IReadOnlyDictionary<Type, IEntitlementStrategy> _strategies;

    public EntitlementService(
        IPurchaseService purchaseService,
        IProductRepository productRepository,
        IEnumerable<IEntitlementStrategy> strategies)
    {
        _purchaseService = purchaseService;
        _productRepository = productRepository;
        _strategies = strategies.ToDictionary(s => s.ProductDomainType);
    }

    public async Task<bool> CanWatchMovieAsync(Guid userId, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default)
    {
        var purchases = await _purchaseService.GetHistoryAsync(userId, cancellationToken);

        foreach (var purchase in purchases)
        {
            if (!string.Equals(purchase.Status, "Completed", StringComparison.OrdinalIgnoreCase)) continue;

            foreach (var item in purchase.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null) continue;

                if (_strategies.TryGetValue(product.GetType(), out var strategy))
                {
                    var hasAccess = await strategy.GrantsAccessAsync(item.ProductId, purchase.DateUtc, festivalId, festivalFilmId, sessionId, cancellationToken);
                    if (hasAccess) return true;
                }
            }
        }
        return false;
    }

    public async Task<bool> HasPurchasedForSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var purchases = await _purchaseService.GetHistoryAsync(userId, cancellationToken);

        foreach (var purchase in purchases)
        {
            if (!string.Equals(purchase.Status, "Completed", StringComparison.OrdinalIgnoreCase)) continue;

            foreach (var item in purchase.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is Ticket ticket && ticket.SessionId == sessionId)
                {
                    return true;
                }
            }
        }
        return false;
    }
}