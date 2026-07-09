// -----------------------------------------------------------------------------
// Awards, nominations & votes — Nominee picker page (four GUID slots + candidate options).
// -----------------------------------------------------------------------------
namespace UmaFestHub.Web.ViewModels;

public sealed class AwardNomineesPageViewModel
{
	public Guid FestivalId { get; set; }
	public string? AwardName { get; set; }
	public string Category { get; set; } = string.Empty;
	public int CategoryValue { get; set; }
	public IReadOnlyList<NomineeOptionViewModel> Options { get; set; } = Array.Empty<NomineeOptionViewModel>();
	public string? EndDate { get; set; }
	public DateTime EndDateUtc { get; set; }
	public string? ErrorMessage { get; set; }
	public Guid? SelectedNominee1 { get; set; }
	public Guid? SelectedNominee2 { get; set; }
	public Guid? SelectedNominee3 { get; set; }
	public Guid? SelectedNominee4 { get; set; }
}

public sealed class NomineeOptionViewModel
{
	public Guid Id { get; set; }
	public string Label { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
}

public static class AwardNomineeValidationMessages
{
	public const string RepeatedNominees =
		"You have repeated nominees. Every nominee option should be different";

	public const string InsufficientNomineeOptions =
		"You should at least have four different options per nominee to create an Award.";
}

