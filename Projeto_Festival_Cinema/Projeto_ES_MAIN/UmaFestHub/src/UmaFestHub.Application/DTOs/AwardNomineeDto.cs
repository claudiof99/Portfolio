// -----------------------------------------------------------------------------
// Awards, nominations & votes — Per-nominee row (label, vote counts, optional image).
// -----------------------------------------------------------------------------
namespace UmaFestHub.Application.DTOs;

public sealed record AwardNomineeDto(Guid NominationId, string Label, int VoteCount, int VotePercentage, string? ImageUrl = null);
