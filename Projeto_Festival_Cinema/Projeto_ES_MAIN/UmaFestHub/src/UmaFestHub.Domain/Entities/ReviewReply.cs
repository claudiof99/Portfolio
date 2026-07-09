// -----------------------------------------------------------------------------
// Review replies — domain entity for a text response under a Review; mirrors review
// moderation (status, report flags, admin hide). Factory: Create(...).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Domain.Entities;

/// <summary>
/// A text response to a <see cref="Review"/> (not a star rating; parent owns film context).
/// Moderation flags mirror <see cref="Review"/> (status, report, hide).
/// </summary>
public class ReviewReply
{
	public const int MaxCommentLength = 1200;

	public Guid Id { get; set; }
	public Guid ReviewId { get; set; }
	public Guid UserId { get; set; }
	public string Comment { get; set; } = string.Empty;
	public DateTime DateUtc { get; set; } = DateTime.UtcNow;
	public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
	public bool IsReported { get; set; }
	public bool HasBeenReported { get; set; }
	public bool IsHiddenByAdmin { get; set; }

	public Review? Review { get; set; }
	public User? User { get; set; }

	public static ReviewReply Create(Guid userId, Guid reviewId, string comment, ReviewStatus initialStatus)
	{
		if (userId == Guid.Empty)
		{
			throw new ArgumentException("UserId is required.", nameof(userId));
		}

		if (reviewId == Guid.Empty)
		{
			throw new ArgumentException("ReviewId is required.", nameof(reviewId));
		}

		var normalized = (comment ?? string.Empty).Trim();
		if (string.IsNullOrWhiteSpace(normalized))
		{
			throw new ArgumentException("Comment is required.", nameof(comment));
		}

		if (normalized.Length > MaxCommentLength)
		{
			throw new ArgumentException($"Comment must be {MaxCommentLength} characters or less.", nameof(comment));
		}

		return new ReviewReply
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			ReviewId = reviewId,
			Comment = normalized,
			DateUtc = DateTime.UtcNow,
			Status = initialStatus,
			IsReported = false,
			HasBeenReported = false,
			IsHiddenByAdmin = false
		};
	}
}
