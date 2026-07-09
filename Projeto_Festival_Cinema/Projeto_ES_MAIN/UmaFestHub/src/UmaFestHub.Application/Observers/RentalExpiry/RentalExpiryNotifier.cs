using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.RentalExpiry;

/// <summary>
/// Fans out each <see cref="RentalExpiryContext"/> to every <see cref="IRentalExpiryObserver"/> (scheduled pass per qualifying rental line).
/// </summary>
public sealed class RentalExpiryNotifier : IRentalExpiryNotifier
{
    private readonly IEnumerable<IRentalExpiryObserver> _observers;
    private readonly ILogger<RentalExpiryNotifier> _logger;

    public RentalExpiryNotifier(
        IEnumerable<IRentalExpiryObserver> observers,
        ILogger<RentalExpiryNotifier> logger)
    {
        _observers = observers;
        _logger = logger;
    }

    public async Task NotifyAsync(RentalExpiryContext context, CancellationToken cancellationToken = default)
    {
        // Register more IRentalExpiryObserver implementations for parallel channels (email, push) if needed.
        foreach (var observer in _observers)
        {
            try
            {
                await observer.OnRentalExpiringAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Observer {Observer} failed for rental {RentalId}",
                    observer.GetType().Name, context.RentalId);
            }
        }
    }
}
