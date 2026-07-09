// -----------------------------------------------------------------------------
// Awards, nominations & votes — Film category: nominee must be a festival film row.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Services.Validators;

public sealed class FilmValidator : INominationValidator
{
	public AwardCategory Category => AwardCategory.Film;

	public Task<(bool Valid, string? Error)> ValidateAsync(AwardNomination nomination, CancellationToken ct = default)
	{
		if (nomination.CreditFilmId is not null)
		{
			return Task.FromResult<(bool, string?)>((false, "Nomination_FilmCannotReferenceCredit"));
		}

		if (nomination.FestivalFilmId is null || nomination.FestivalFilmId == Guid.Empty)
		{
			return Task.FromResult<(bool, string?)>((false, "Nomination_FilmMustReferenceFestivalFilm"));
		}

		return Task.FromResult<(bool, string?)>((true, null));
	}
}
