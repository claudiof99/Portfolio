// -----------------------------------------------------------------------------
// Awards, nominations & votes — Base for Actor/Director/Writing: credit + role name.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services.Validators;

public abstract class RoleBasedCreditValidator : INominationValidator
{
	protected abstract string RequiredRole { get; }
	protected abstract string WrongRoleKey { get; }
	public abstract AwardCategory Category { get; }

	public Task<(bool Valid, string? Error)> ValidateAsync(AwardNomination nomination, CancellationToken ct = default)
	{
		if (nomination.FestivalFilmId is not null)
		{
			return Task.FromResult<(bool, string?)>((false, "Nomination_CreditCannotReferenceFestivalFilm"));
		}

		if (nomination.CreditFilmId is null || nomination.CreditFilmId == Guid.Empty)
		{
			return Task.FromResult<(bool, string?)>((false, "Nomination_MustReferenceCredit"));
		}

		// In the web flow, nominees are picked from a pre-filtered candidates list per category/role.
		// So at creation time we may only have CreditFilmId (without the navigation loaded).
		if (nomination.CreditFilm is null)
		{
			return Task.FromResult<(bool, string?)>((true, null));
		}

		var role = nomination.CreditFilm?.Role;
		if (string.IsNullOrWhiteSpace(role))
		{
			return Task.FromResult<(bool, string?)>((false, "Nomination_CreditRoleMissing"));
		}

		if (!string.Equals(role, RequiredRole, StringComparison.OrdinalIgnoreCase))
		{
			return Task.FromResult<(bool, string?)>((false, WrongRoleKey));
		}

		return Task.FromResult<(bool, string?)>((true, null));
	}
}
