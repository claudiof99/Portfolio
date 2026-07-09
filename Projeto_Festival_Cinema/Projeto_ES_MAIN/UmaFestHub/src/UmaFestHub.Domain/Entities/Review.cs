namespace UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

// Review aggregate persisted in the database.
// Contains moderation/reporting flags that drive visibility and staff workflows.
// Replies: collection Replies (ReviewReply) for the thread under this card.
public class Review
{
	public const int MinRating = 1;
	public const int MaxRating = 5;
	public const int MaxCommentLength = 1200;

	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
	public int Rating { get; set; }
	public string Comment { get; set; } = string.Empty;
	public DateTime DateUtc { get; set; } = DateTime.UtcNow;
	public int ExternalFilmId { get; set; }
	public Guid? FestivalFilmId { get; set; }
	public Guid? FilmId { get; set; }
	public bool IsReported { get; set; }
	public bool HasBeenReported { get; set; }
	public bool IsHiddenByAdmin { get; set; }

	public User? User { get; set; }
	public FestivalFilm? FestivalFilm { get; set; }
	public Film? Film { get; set; }

	/// <summary>Threaded text replies to this review (see <see cref="ReviewReply"/>).</summary>
	public ICollection<ReviewReply> Replies { get; set; } = new List<ReviewReply>();

	public static Review Create(
		Guid userId,
		Guid? festivalFilmId,
		Guid? filmId,
		int externalFilmId,
		int rating,
		string comment,
		ReviewStatus status,
		DateTime dateUtc)
	{
		if (userId == Guid.Empty)
		{
			throw new ArgumentException("UserId is required.", nameof(userId));
		}

		if (rating is < MinRating or > MaxRating)
		{
			throw new ArgumentOutOfRangeException(nameof(rating), $"Rating must be between {MinRating} and {MaxRating}.");
		}

		var normalizedComment = (comment ?? string.Empty).Trim();
		if (string.IsNullOrWhiteSpace(normalizedComment))
		{
			throw new ArgumentException("Comment is required.", nameof(comment));
		}

		if (normalizedComment.Length > MaxCommentLength)
		{
			throw new ArgumentException($"Comment must be {MaxCommentLength} characters or less.", nameof(comment));
		}

		return new Review
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			FestivalFilmId = festivalFilmId,
			ExternalFilmId = externalFilmId,
			FilmId = filmId,
			Rating = rating,
			Comment = normalizedComment,
			DateUtc = dateUtc,
			Status = status,
			IsReported = false,
			HasBeenReported = false,
			IsHiddenByAdmin = false
		};
	}
}
