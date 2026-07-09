namespace UmaFestHub.Application.Observers.PurchaseCompleted;

/// <summary>Entry point called from <see cref="Services.PurchaseService"/> to notify all purchase-completed observers.</summary>
public interface IPurchaseCompletedNotifier
{
    /// <summary>Runs every <see cref="IPurchaseCompletedObserver"/> for this checkout.</summary>
    Task NotifyAsync(PurchaseCompletedContext context, CancellationToken cancellationToken = default);
}
