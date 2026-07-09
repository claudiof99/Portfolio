using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.PurchaseCompleted;

/// <summary>
/// Fans out <see cref="PurchaseCompletedContext"/> to every <see cref="IPurchaseCompletedObserver"/> after checkout (e.g. in-app confirmation).
/// </summary>
public sealed class PurchaseCompletedNotifier : IPurchaseCompletedNotifier
{
    private readonly IEnumerable<IPurchaseCompletedObserver> _observers;
    private readonly ILogger<PurchaseCompletedNotifier> _logger;

    public PurchaseCompletedNotifier(
        IEnumerable<IPurchaseCompletedObserver> observers,
        ILogger<PurchaseCompletedNotifier> logger)
    {
        _observers = observers;
        _logger = logger;
    }

    public async Task NotifyAsync(PurchaseCompletedContext context, CancellationToken cancellationToken = default)
    {
        // Add more IPurchaseCompletedObserver implementations in DI for parallel side effects (email, analytics).
        foreach (var observer in _observers)
        {
            try
            {
                await observer.OnPurchaseCompletedAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Observer {Observer} failed for purchase {PurchaseId}",
                    observer.GetType().Name, context.PurchaseId);
            }
        }
    }
}
