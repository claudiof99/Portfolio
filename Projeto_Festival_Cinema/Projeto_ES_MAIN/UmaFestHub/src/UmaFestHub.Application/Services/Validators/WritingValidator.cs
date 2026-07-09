// -----------------------------------------------------------------------------
// Awards, nominations & votes — Writing category; TMDB role matched as "Writer".
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services.Validators;

public sealed class WritingValidator : RoleBasedCreditValidator
{
	protected override string RequiredRole => "Writer";
	protected override string WrongRoleKey => "Nomination_MustBeWriter";

	public override AwardCategory Category => AwardCategory.Writing;
}
