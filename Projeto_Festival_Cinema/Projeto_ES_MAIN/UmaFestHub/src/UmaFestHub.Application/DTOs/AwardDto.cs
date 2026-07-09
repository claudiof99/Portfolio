// -----------------------------------------------------------------------------
// Awards, nominations & votes — DTO for award list/detail and create payloads.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Application.DTOs;

public sealed class AwardDto
{
	public Guid Id { get; set; }
	public Guid FestivalId { get; set; }
	public string FestivalName { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public int NominationCount { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime EndDateUtc { get; set; }
	public int DaysRemaining { get; set; }
	public IReadOnlyList<AwardNomineeDto> Nominees { get; set; } = Array.Empty<AwardNomineeDto>();
}
