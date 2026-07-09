namespace UmaFestHub.Application.Validators;

public record CartValidationResult(
	bool IsValid,
	IReadOnlyList<string> Errors = null!)
{
	public static CartValidationResult Success() => new(true);

	public static CartValidationResult Failure(params string[] errors)
		=> new(false, errors.ToList());

	public static CartValidationResult Failure(IEnumerable<string> errors)
		=> new(false, errors.ToList());
}