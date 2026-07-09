using System.Text.Json;
using System.Text.Json.Serialization;

namespace UmaFestHub.Application.DTOs;

public static class NotificationKinds
{
	public const string ReviewPending = "review-pending";
	public const string ReviewOutcome = "review-outcome";
	public const string ReplyPending = "reply-pending";
	public const string ReplyOutcome = "reply-outcome";
	public const string AwardResults = "award-results";
	public const string FestivalEnding = "festival-ending";
	public const string RentalExpiring = "rental-expiring";
	public const string PurchaseCompleted = "purchase-completed";
}

public sealed record NotificationAwardResultLine(string Label, int Percent);

/// <summary>Serializable notification template stored in DB and rendered at display time in the user's culture.</summary>
public sealed class NotificationTemplate
{
	public required string Kind { get; init; }
	public string? CollapseGroup { get; init; }

	public bool? IsApproved { get; init; }
	public string? FestivalName { get; init; }
	public string? FilmTitle { get; init; }
	public int? Rating { get; init; }
	public string? Comment { get; init; }

	public string? AwardName { get; init; }
	public NotificationAwardResultLine[]? Results { get; init; }

	public DateTime? EndDateUtc { get; init; }
	public bool? UseDefaultFestivalName { get; init; }

	public DateTime? ExpiresAtUtc { get; init; }
	public bool? UseDefaultFilmTitle { get; init; }

	public decimal? TotalAmount { get; init; }

	public static NotificationTemplate ReviewPending() => new()
	{
		Kind = NotificationKinds.ReviewPending,
		CollapseGroup = NotificationKinds.ReviewPending,
	};

	public static NotificationTemplate ReviewOutcome(
		bool isApproved,
		string festivalName,
		string filmTitle,
		int rating,
		string comment) => new()
	{
		Kind = NotificationKinds.ReviewOutcome,
		IsApproved = isApproved,
		FestivalName = festivalName,
		FilmTitle = filmTitle,
		Rating = rating,
		Comment = comment,
	};

	public static NotificationTemplate ReplyPending() => new()
	{
		Kind = NotificationKinds.ReplyPending,
		CollapseGroup = NotificationKinds.ReplyPending,
	};

	public static NotificationTemplate ReplyOutcome(
		bool isApproved,
		string festivalName,
		string filmTitle,
		string comment) => new()
	{
		Kind = NotificationKinds.ReplyOutcome,
		IsApproved = isApproved,
		FestivalName = festivalName,
		FilmTitle = filmTitle,
		Comment = comment,
	};

	public static NotificationTemplate AwardResults(string awardName, IReadOnlyList<NotificationAwardResultLine> results) => new()
	{
		Kind = NotificationKinds.AwardResults,
		AwardName = awardName,
		Results = results.ToArray(),
	};

	public static NotificationTemplate FestivalEnding(string? festivalName, DateTime endDateUtc) => new()
	{
		Kind = NotificationKinds.FestivalEnding,
		FestivalName = festivalName,
		EndDateUtc = endDateUtc,
		UseDefaultFestivalName = string.IsNullOrWhiteSpace(festivalName),
	};

	public static NotificationTemplate RentalExpiring(string? filmTitle, DateTime expiresAtUtc) => new()
	{
		Kind = NotificationKinds.RentalExpiring,
		FilmTitle = filmTitle,
		ExpiresAtUtc = expiresAtUtc,
		UseDefaultFilmTitle = string.IsNullOrWhiteSpace(filmTitle),
	};

	public static NotificationTemplate PurchaseCompleted(decimal totalAmount) => new()
	{
		Kind = NotificationKinds.PurchaseCompleted,
		TotalAmount = totalAmount,
	};
}

public static class NotificationTemplateJson
{
	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	public static string Serialize(NotificationTemplate template)
		=> JsonSerializer.Serialize(template, Options);

	public static NotificationTemplate? Deserialize(string? json)
		=> string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<NotificationTemplate>(json, Options);
}
