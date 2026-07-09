// In-app notifications: one undelivered row returned by GET /notifications/pending for replay after login.
namespace UmaFestHub.Application.DTOs;

/// <summary>Unread queued notification row for the current user (API + client modal).</summary>
public sealed record PendingNotificationItemDto(
	Guid Id,
	string Title,
	string Message,
	string? CorrelationId,
	DateTime CreatedUtc,
	string? TemplateJson = null,
	string? CollapseGroup = null);
