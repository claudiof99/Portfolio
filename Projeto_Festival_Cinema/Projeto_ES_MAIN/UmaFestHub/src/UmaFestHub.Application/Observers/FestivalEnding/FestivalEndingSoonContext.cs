namespace UmaFestHub.Application.Observers.FestivalEnding;

/// <summary>Payload passed from the reminder orchestrator to observers for one enqueue attempt.</summary>
/// <param name="FestivalId">Festival whose <see cref="EndDateUtc"/> is within the reminder window.</param>
/// <param name="FestivalName">Shown in UI; may be blank (observer substitutes a fallback).</param>
/// <param name="EndDateUtc">Exact end instant used for countdown copy (“less than N day(s) remaining”).</param>
/// <param name="UserId">Subscriber with at least one completed purchase for this festival.</param>
public sealed record FestivalEndingSoonContext(
	Guid FestivalId,
	string FestivalName,
	DateTime EndDateUtc,
	Guid UserId);
