using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Infrastructure.Payment;

public class SimulatedPaymentService : IPaymentSimulationService
{
	public Task<PaymentResultDto> ProcessAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
	{
		if (amount < 0)
		{
			return Task.FromResult(new PaymentResultDto(false, string.Empty, "Amount cannot be negative."));
		}

		var transactionId = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";
		return Task.FromResult(new PaymentResultDto(true, transactionId, null));
	}
}
