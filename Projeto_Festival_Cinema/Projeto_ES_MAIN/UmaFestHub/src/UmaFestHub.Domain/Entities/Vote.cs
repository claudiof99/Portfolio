// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain
// Customer vote for one nomination; composite key (UserId, AwardNominationId) in EF.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Domain.Entities;

public class Vote
{
    // Non-key surrogate identifier (DB still enforces composite PK on (UserId, AwardNominationId))
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AwardNominationId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AwardNomination? AwardNomination { get; set; }
    public User? User { get; set; }
}