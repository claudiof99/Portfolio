using UmaFestHub.Application.Messaging;

namespace UmaFestHub.Application.Validators.Purchase;

public sealed class PurchaseValidationResult
{
	public bool IsValid { get; }
	public IReadOnlyList<UserMessage> Errors { get; }

	private PurchaseValidationResult(bool isValid, IReadOnlyList<UserMessage> errors)
	{
		IsValid = isValid;
		Errors = errors;
	}

	public static PurchaseValidationResult Success()
		=> new(true, Array.Empty<UserMessage>());

	public static PurchaseValidationResult Failure(params UserMessage[] errors)
		=> new(false, errors.ToList());

	public static PurchaseValidationResult Failure(IEnumerable<UserMessage> errors)
		=> new(false, errors.ToList());
}
