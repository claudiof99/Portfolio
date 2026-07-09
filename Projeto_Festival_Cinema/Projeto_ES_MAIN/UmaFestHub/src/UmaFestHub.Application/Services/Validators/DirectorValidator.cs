// -----------------------------------------------------------------------------
// Awards, nominations & votes — Director category uses RoleBasedCreditValidator.
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services.Validators;

public sealed class DirectorValidator : RoleBasedCreditValidator
{
	protected override string RequiredRole => "Director";
	protected override string WrongRoleKey => "Nomination_MustBeDirector";

	public override AwardCategory Category => AwardCategory.Director;
}
