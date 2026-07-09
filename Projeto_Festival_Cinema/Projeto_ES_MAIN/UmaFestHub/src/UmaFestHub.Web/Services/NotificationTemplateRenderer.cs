using System.Text;
using Microsoft.Extensions.Localization;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.Services;

public sealed class NotificationTemplateRenderer(IStringLocalizer<SharedResources> localizer) : INotificationTemplateRenderer
{
	private const int AuthorModalCommentMaxChars = 600;

	public (string Title, string Message) Render(NotificationTemplate template)
	{
		return template.Kind switch
		{
			NotificationKinds.ReviewPending => (
				localizer["Notification_ReviewPendingTitle"].Value,
				localizer["Notification_ReviewPendingMessage"].Value),
			NotificationKinds.ReviewOutcome => RenderReviewOutcome(template),
			NotificationKinds.ReplyPending => (
				localizer["Notification_ReplyPendingTitle"].Value,
				localizer["Notification_ReplyPendingMessage"].Value),
			NotificationKinds.ReplyOutcome => RenderReplyOutcome(template),
			NotificationKinds.AwardResults => RenderAwardResults(template),
			NotificationKinds.FestivalEnding => RenderFestivalEnding(template),
			NotificationKinds.RentalExpiring => RenderRentalExpiring(template),
			NotificationKinds.PurchaseCompleted => RenderPurchaseCompleted(template),
			_ => (string.Empty, string.Empty),
		};
	}

	private (string Title, string Message) RenderReviewOutcome(NotificationTemplate template)
	{
		var isApproved = template.IsApproved == true;
		var title = isApproved
			? localizer["Notification_ReviewApprovedTitle"].Value
			: localizer["Notification_ReviewRejectedTitle"].Value;
		var outcome = isApproved
			? localizer["Review_StatusApproved"].Value
			: localizer["Review_StatusRejected"].Value;
		var comment = TrimForModal(template.Comment ?? string.Empty, AuthorModalCommentMaxChars);

		var body = new StringBuilder();
		body.AppendLine(localizer["Notification_ReviewOutcomeResponse", outcome].Value);
		body.AppendLine(localizer["Notification_ReviewOutcomeFestival", template.FestivalName ?? string.Empty].Value);
		body.AppendLine(localizer["Notification_ReviewOutcomeFilm", template.FilmTitle ?? string.Empty].Value);
		body.AppendLine(localizer["Notification_ReviewOutcomeRating", template.Rating ?? 0].Value);
		body.AppendLine();
		body.AppendLine(localizer["Notification_ReviewOutcomeYourReview"].Value);
		body.Append(comment);

		return (title, body.ToString());
	}

	private (string Title, string Message) RenderReplyOutcome(NotificationTemplate template)
	{
		var isApproved = template.IsApproved == true;
		var title = isApproved
			? localizer["Notification_ReplyApprovedTitle"].Value
			: localizer["Notification_ReplyRejectedTitle"].Value;
		var outcome = isApproved
			? localizer["Review_StatusApproved"].Value
			: localizer["Review_StatusRejected"].Value;
		var comment = TrimForModal(template.Comment ?? string.Empty, AuthorModalCommentMaxChars);

		var body = new StringBuilder();
		body.AppendLine(localizer["Notification_ReviewOutcomeResponse", outcome].Value);
		body.AppendLine(localizer["Notification_ReviewOutcomeFestival", template.FestivalName ?? string.Empty].Value);
		body.AppendLine(localizer["Notification_ReviewOutcomeFilm", template.FilmTitle ?? string.Empty].Value);
		body.AppendLine();
		body.AppendLine(localizer["Notification_ReplyOutcomeYourReply"].Value);
		body.Append(comment);

		return (title, body.ToString());
	}

	private (string Title, string Message) RenderAwardResults(NotificationTemplate template)
	{
		var title = localizer["Notification_AwardVotingClosedTitle", template.AwardName ?? string.Empty].Value;
		var body = new StringBuilder();
		body.AppendLine(localizer["Notification_AwardFinalResults"].Value);
		foreach (var line in template.Results ?? [])
		{
			body.AppendLine(localizer["Notification_AwardResultLine", localizer.LocalizeDisplayText(line.Label), line.Percent].Value);
		}

		return (title, body.ToString().TrimEnd());
	}

	private (string Title, string Message) RenderFestivalEnding(NotificationTemplate template)
	{
		var title = localizer["Notification_FestivalEndingTitle"].Value;
		var endDate = template.EndDateUtc ?? DateTime.UtcNow;
		var safeName = template.UseDefaultFestivalName == true || string.IsNullOrWhiteSpace(template.FestivalName)
			? localizer["Notification_FestivalEndingDefaultName"].Value
			: template.FestivalName.Trim();
		var remainingPhrase = DescribeLessThanDaysRemaining(endDate, DateTime.UtcNow);
		var message = localizer[
			"Notification_FestivalEndingMessage",
			safeName,
			endDate.ToString("yyyy-MM-dd"),
			remainingPhrase].Value;
		return (title, message);
	}

	private (string Title, string Message) RenderRentalExpiring(NotificationTemplate template)
	{
		var title = localizer["Notification_RentalExpiringTitle"].Value;
		var expiresAt = template.ExpiresAtUtc ?? DateTime.UtcNow;
		var film = template.UseDefaultFilmTitle == true || string.IsNullOrWhiteSpace(template.FilmTitle)
			? localizer["Notification_RentalDefaultFilm"].Value
			: template.FilmTitle.Trim();
		var remainingPhrase = DescribeTimeRemaining(expiresAt, DateTime.UtcNow);
		var message = localizer[
			"Notification_RentalExpiringMessage",
			film,
			expiresAt.ToString("yyyy-MM-dd HH:mm"),
			remainingPhrase].Value;
		return (title, message);
	}

	private (string Title, string Message) RenderPurchaseCompleted(NotificationTemplate template)
	{
		var amount = template.TotalAmount ?? 0m;
		var formatted = amount.FormatCurrency();
		return (
			localizer["Notification_PurchaseCompletedTitle"].Value,
			localizer["Notification_PurchaseCompletedMessage", formatted].Value);
	}

	private string DescribeLessThanDaysRemaining(DateTime endDateUtc, DateTime utcNow)
	{
		var remaining = endDateUtc - utcNow;
		if (remaining <= TimeSpan.Zero)
		{
			return localizer["Notification_EndingVerySoon"].Value;
		}

		var ceilDays = (int)Math.Ceiling(remaining.TotalDays);
		ceilDays = Math.Clamp(ceilDays, 1, 3);
		return ceilDays == 1
			? localizer["Notification_LessThan1Day"].Value
			: localizer["Notification_LessThanDays", ceilDays].Value;
	}

	private string DescribeTimeRemaining(DateTime expiresAtUtc, DateTime utcNow)
	{
		var remaining = expiresAtUtc - utcNow;
		if (remaining <= TimeSpan.Zero)
		{
			return localizer["Notification_EndingVerySoon"].Value;
		}

		if (remaining.TotalHours < 24)
		{
			var hours = Math.Max(1, (int)Math.Ceiling(remaining.TotalHours));
			return hours == 1
				? localizer["Notification_LessThan1Hour"].Value
				: localizer["Notification_LessThanHours", hours].Value;
		}

		var ceilDays = (int)Math.Ceiling(remaining.TotalDays);
		ceilDays = Math.Clamp(ceilDays, 1, 3);
		return ceilDays == 1
			? localizer["Notification_LessThan1Day"].Value
			: localizer["Notification_LessThanDays", ceilDays].Value;
	}

	private static string TrimForModal(string text, int maxChars)
	{
		var t = text.Trim();
		if (t.Length <= maxChars)
		{
			return t;
		}

		return t[..maxChars].TrimEnd() + "…";
	}
}
