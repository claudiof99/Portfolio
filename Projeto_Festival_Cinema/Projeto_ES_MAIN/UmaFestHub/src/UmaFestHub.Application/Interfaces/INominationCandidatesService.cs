// -----------------------------------------------------------------------------
// Awards, nominations & votes — Candidate options for nominee selection UI.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces;

public interface INominationCandidatesService
{
	Task<IReadOnlyList<NomineeOptionDto>> GetCandidatesAsync(AwardCategory category, Guid festivalId, CancellationToken ct = default);
}

