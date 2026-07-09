namespace UmaFestHub.Application.Observers.PurchaseCompleted;

/// <summary>Hook for post-checkout side effects; default implementation sends the purchase-completed in-app notification.</summary>
public interface IPurchaseCompletedObserver
{
    /// <summary>Invoked once per successful checkout after the purchase row is saved.</summary>
    Task OnPurchaseCompletedAsync(PurchaseCompletedContext context, CancellationToken cancellationToken = default);
}
