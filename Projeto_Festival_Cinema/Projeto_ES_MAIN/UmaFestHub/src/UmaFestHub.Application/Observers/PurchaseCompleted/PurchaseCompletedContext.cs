namespace UmaFestHub.Application.Observers.PurchaseCompleted;

/// <summary>Payload for purchase-success observers (built in <see cref="Services.PurchaseService.CheckoutAsync"/>).</summary>
public sealed class PurchaseCompletedContext
{
    /// <summary>Buyer receiving the notification.</summary>
    public required Guid UserId { get; init; }
    /// <summary>Persisted purchase id; used in notification correlation <c>purchase-completed:{id}</c>.</summary>
    public required Guid PurchaseId { get; init; }
    /// <summary>Checkout total shown in the modal body.</summary>
    public required decimal TotalAmount { get; init; }
    /// <summary>Purchase timestamp (UTC) when checkout completed.</summary>
    public required DateTime CompletedAt { get; init; }
}
