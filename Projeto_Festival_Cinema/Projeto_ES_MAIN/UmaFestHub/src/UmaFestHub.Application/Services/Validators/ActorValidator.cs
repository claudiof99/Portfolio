// -----------------------------------------------------------------------------
// Awards, nominations & votes — Actor category uses RoleBasedCreditValidator ("Actor").
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services.Validators;

public sealed class ActorValidator : RoleBasedCreditValidator
{
	protected override string RequiredRole => "Actor";
	protected override string WrongRoleKey => "Nomination_MustBeActor";

	public override AwardCategory Category => AwardCategory.Actor;
}
