// -----------------------------------------------------------------------------
// Awards, nominations & votes — Strategy per AwardCategory (film vs credit rules).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces;

public interface INominationValidator
{
	AwardCategory Category { get; }

	Task<(bool Valid, string? Error)> ValidateAsync(
		AwardNomination nomination,
		CancellationToken ct = default);
}

