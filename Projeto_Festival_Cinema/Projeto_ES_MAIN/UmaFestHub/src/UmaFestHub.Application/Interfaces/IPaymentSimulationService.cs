using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IPaymentSimulationService
{
	Task<PaymentResultDto> ProcessAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default);
}
