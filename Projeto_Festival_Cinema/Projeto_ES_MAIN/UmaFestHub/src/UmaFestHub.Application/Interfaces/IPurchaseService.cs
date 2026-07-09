using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IPurchaseService
{
	Task<IReadOnlyList<PurchaseDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<Guid> CheckoutAsync(Guid userId, IReadOnlyList<PurchaseItemDto> items, CancellationToken cancellationToken = default);
}
