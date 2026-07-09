using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Validators.Purchase;

public sealed class TicketValidator : IPurchaseValidator
{
	private readonly ISessionRepository _sessionRepository;

	public string ProductType => nameof(Ticket);

	public TicketValidator(ISessionRepository sessionRepository)
	{
		_sessionRepository = sessionRepository;
	}

	public async Task<PurchaseValidationResult> ValidateAsync(Guid userId, Product product, CancellationToken cancellationToken = default)
	{
		if (product is not Ticket ticket)
		{
			return PurchaseValidationResult.Success();
		}

		var session = await _sessionRepository.GetByIdAsync(ticket.SessionId, cancellationToken);
		if (session is null)
		{
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_SessionNotFound));
		}

		if (session.StartTimeUtc <= DateTime.UtcNow)
		{
			return PurchaseValidationResult.Failure(new UserMessage(UserMessageKeys.Purchase_SessionAlreadyStarted));
		}

		return PurchaseValidationResult.Success();
	}
}
