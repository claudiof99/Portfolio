// In-app notifications: client-facing shape (pre-rendered in the user's culture).
namespace UmaFestHub.Application.DTOs;

/// <summary>Notification row for the browser modal (rendered client-side from <see cref="TemplateJson"/> in the user's culture).</summary>
public sealed record ClientNotificationDto(
	Guid Id,
	string Title,
	string Message,
	string? CorrelationId,
	string? CollapseGroup = null,
	string? TemplateJson = null);
