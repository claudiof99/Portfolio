// In-app notifications: payload for “voting closed” fan-out to voters (except deactivating user).
namespace UmaFestHub.Application.Observers.Awards;

public sealed record AwardResultLine(string Label, int Percent);

/// <summary>Published when an organizer closes voting on an award (<see cref="UmaFestHub.Domain.Entities.Award.IsActive"/> set false).</summary>
public sealed record AwardVotingClosedContext(
	Guid AwardId,
	string AwardName,
	Guid DeactivatedByUserId,
	IReadOnlyList<AwardResultLine> Results,
	IReadOnlyList<Guid> VoterUserIds);
