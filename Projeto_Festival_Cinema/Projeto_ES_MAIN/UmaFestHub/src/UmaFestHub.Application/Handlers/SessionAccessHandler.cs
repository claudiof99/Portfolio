using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Handlers;

public abstract class SessionAccessHandler
{
    private SessionAccessHandler? _next;

    public virtual Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto sessionAccessDto,
        CancellationToken cancellationToken = default)
    {
        if (_next is not null)
        {
            return _next.HandleAsync(sessionAccessDto, cancellationToken);
        }

        return Task.FromResult<(bool, UserMessage?)>((false, new UserMessage(UserMessageKeys.SessionAccess_Denied)));
    }

    public SessionAccessHandler SetNext(SessionAccessHandler handler)
    {
        _next = handler;
        return handler;
    }
}

public sealed class SessionExistsHandler(ISessionRepository sessionRepository) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto sessionAccessDto,
        CancellationToken cancellationToken = default)
    {
        if (!sessionAccessDto.SessionId.HasValue || sessionAccessDto.SessionId == Guid.Empty)
        {
            return await base.HandleAsync(sessionAccessDto, cancellationToken);
        }

        var session = await sessionRepository.GetByIdAsync(sessionAccessDto.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return (false, new UserMessage(UserMessageKeys.SessionAccess_SessionNotFound));
        }

        return await base.HandleAsync(sessionAccessDto, cancellationToken);
    }
}

/// <summary>Time-gates FixedSession and PremierSession: watching is only allowed during the session window.</summary>
public sealed class SessionTimeGateHandler(ISessionRepository sessionRepository) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto context,
        CancellationToken cancellationToken = default)
    {
        if (!context.SessionId.HasValue || context.SessionId == Guid.Empty)
        {
            // No specific session (pass/rental access) — skip time-gating
            return await base.HandleAsync(context, cancellationToken);
        }

        var session = await sessionRepository.GetByIdAsync(context.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return await base.HandleAsync(context, cancellationToken);
        }

        // Only time-gate FixedSession and PremierSession (not AccessWindow which has its own handler)
        var sessionType = session.SessionType ?? string.Empty;
        var isTimeBound = sessionType.Contains("Fixed", StringComparison.OrdinalIgnoreCase)
                       || sessionType.Contains("Premier", StringComparison.OrdinalIgnoreCase);

        if (!isTimeBound)
        {
            return await base.HandleAsync(context, cancellationToken);
        }

        if (context.NowUtc < context.SessionStartUtc)
        {
            return (false, new UserMessage(
                UserMessageKeys.SessionAccess_NotStartedYet,
                context.SessionStartUtc.ToString("dd MMM yyyy, HH:mm")));
        }

        if (context.NowUtc > context.SessionEndUtc)
        {
            return (false, new UserMessage(UserMessageKeys.SessionAccess_SessionEnded));
        }

        return await base.HandleAsync(context, cancellationToken);
    }
}

public sealed class AccessWindowAccessHandler(
    ISessionRepository sessionRepository,
    IPurchaseService purchaseService,
    IProductService productService) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto context,
        CancellationToken cancellationToken = default)
    {
        if (!context.SessionId.HasValue || context.SessionId == Guid.Empty)
        {
            return await base.HandleAsync(context, cancellationToken);
        }

        var session = await sessionRepository.GetByIdAsync(context.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return (false, new UserMessage(UserMessageKeys.SessionAccess_SessionNotFound));
        }

        if (session.SessionType != SessionType.AccessWindow)
        {
            return await base.HandleAsync(context, cancellationToken);
        }

        if (context.NowUtc < context.SessionStartUtc || context.NowUtc > context.SessionEndUtc)
        {
            return (false, new UserMessage(
                UserMessageKeys.SessionAccess_RentalWindow,
                context.SessionStartUtc.ToString("g"),
                context.SessionEndUtc.ToString("g")));
        }

        var rental = await productService.GetRentalDtoAsync(session.FestivalFilmId, cancellationToken);
        if (rental == null)
        {
            return (false, new UserMessage(UserMessageKeys.SessionAccess_NoRentalProduct));
        }

        var history = await purchaseService.GetHistoryAsync(context.UserId, cancellationToken);

        var rentalDuration = TimeSpan.FromHours(
            rental.DurationUnit?.Equals("Days", StringComparison.OrdinalIgnoreCase) == true
                ? rental.DurationValue * 24
                : rental.DurationValue);

        var hasActiveRental = history
            .Where(p => string.Equals(p.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            .Any(p => p.Items.Any(i => i.ProductId == rental.Id)
                   && p.DateUtc + rentalDuration > context.NowUtc);

        if (hasActiveRental)
        {
            return (true, null);
        }

        return await base.HandleAsync(context, cancellationToken);
    }
}

public sealed class CompletePassAccessHandler(
    IPurchaseRepository purchaseRepository,
    IProductRepository productRepository) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto context,
        CancellationToken cancellationToken = default)
    {
        if (context.NowUtc > context.FestivalEndUtc)
        {
            return await base.HandleAsync(context, cancellationToken);
        }

        var purchases = await purchaseRepository.GetByUserIdAsync(context.UserId, cancellationToken);

        var purchasedIds = purchases
            .Where(p => p.Status == PurchaseStatus.Completed)
            .SelectMany(p => p.PurchaseItems.Select(i => i.ProductId))
            .ToHashSet();

        var products = await productRepository.GetByIdsAsync(purchasedIds, cancellationToken);

        var hasCompletePass = products
            .OfType<CompletePass>()
            .Any(p => p.FestivalId == context.FestivalId);

        if (hasCompletePass)
        {
            return (true, null);
        }

        return await base.HandleAsync(context, cancellationToken);
    }
}

public sealed class DailyPassAccessHandler(
    IPurchaseRepository purchaseRepository,
    IProductRepository productRepository) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto context,
        CancellationToken cancellationToken = default)
    {
        var purchases = await purchaseRepository.GetByUserIdAsync(context.UserId, cancellationToken);

        var purchasedIds = purchases
            .Where(p => p.Status == PurchaseStatus.Completed)
            .SelectMany(p => p.PurchaseItems.Select(i => i.ProductId))
            .ToHashSet();

        var products = await productRepository.GetByIdsAsync(purchasedIds, cancellationToken);

        var hasDailyPass = products
            .OfType<DailyPass>()
            .Any(p => p.FestivalId == context.FestivalId
                   && (!context.SessionId.HasValue || p.DateUtc.Date == context.SessionStartUtc.Date));

        if (hasDailyPass)
        {
            return (true, null);
        }

        return await base.HandleAsync(context, cancellationToken);
    }
}

public sealed class UserHasAccessHandler(
    IPurchaseRepository purchaseRepository,
    IProductRepository productRepository) : SessionAccessHandler
{
    public override async Task<(bool Allowed, UserMessage? Error)> HandleAsync(
        SessionAccessDto context,
        CancellationToken cancellationToken = default)
    {
        if (!context.SessionId.HasValue || context.SessionId == Guid.Empty)
        {
            return (false, new UserMessage(UserMessageKeys.SessionAccess_PassRequired));
        }

        var purchasedProductIds = (await purchaseRepository.GetByUserIdAsync(context.UserId, cancellationToken))
            .Where(p => p.Status == PurchaseStatus.Completed)
            .SelectMany(p => p.PurchaseItems.Select(i => i.ProductId))
            .ToHashSet();

        var products = await productRepository.GetByIdsAsync(purchasedProductIds, cancellationToken);

        var hasAccess = products.Any(p => p.GrantsAccessToSession(
            context.SessionId.Value,
            context.FestivalId,
            context.NowUtc,
            context.SessionStartUtc));

        if (hasAccess)
        {
            return (true, null);
        }

        return (false, new UserMessage(UserMessageKeys.SessionAccess_NoValidTicket));
    }
}
