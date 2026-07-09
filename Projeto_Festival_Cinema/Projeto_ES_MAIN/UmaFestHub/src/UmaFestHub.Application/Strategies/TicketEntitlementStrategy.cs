using System;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Strategies;

public class TicketEntitlementStrategy : IEntitlementStrategy
{
    private readonly IProductService _productService;
    private readonly ISessionRepository _sessionRepository;

    public Type ProductDomainType => typeof(Ticket);

    public TicketEntitlementStrategy(IProductService productService, ISessionRepository sessionRepository)
    {
        _productService = productService;
        _sessionRepository = sessionRepository;
    }

    public async Task<bool> GrantsAccessAsync(Guid productId, DateTime purchaseDateUtc, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default)
    {
        if (!sessionId.HasValue) return false;
        var ticket = await _productService.GetTicketDtoAsync(sessionId.Value, cancellationToken);
        if (ticket == null || ticket.Id != productId) return false;

        // Time-gate: only grant access once the session has started
        var session = await _sessionRepository.GetByIdAsync(sessionId.Value, cancellationToken);
        if (session == null) return false;

        return DateTime.UtcNow >= session.StartTimeUtc;
    }
}