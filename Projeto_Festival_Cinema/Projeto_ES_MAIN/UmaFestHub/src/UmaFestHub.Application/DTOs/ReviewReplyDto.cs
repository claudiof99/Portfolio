// -----------------------------------------------------------------------------
// Review replies — DTO crossing Application/Web boundary (maps to ReviewReplyViewModel).
// -----------------------------------------------------------------------------
namespace UmaFestHub.Application.DTOs;

/// <summary>
/// Application boundary shape for a reply row (author name is enriched in the web layer when needed).
/// </summary>
public sealed record ReviewReplyDto(
	Guid Id,
	Guid ReviewId,
	Guid UserId,
	string Comment,
	DateTime DateUtc,
	string Status,
	bool IsReported,
	bool HasBeenReported,
	bool IsHiddenByAdmin);
