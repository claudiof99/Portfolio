// -----------------------------------------------------------------------------
// Awards, nominations & votes — Selectable option when picking four nominees (organizer).
// -----------------------------------------------------------------------------
namespace UmaFestHub.Application.DTOs;

public sealed record NomineeOptionDto(
	Guid Id,
	string Label,
	string? ImageUrl = null);

