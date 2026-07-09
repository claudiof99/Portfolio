// In-app notifications: JSON/SignalR shape sent to the browser for the dismiss-only modal.
namespace UmaFestHub.Application.DTOs;

/// <summary>Client payload for real-time notifications (SignalR / modal UI). Always dismiss-only (OK to acknowledge).</summary>
public sealed record NotificationPayloadDto(
	string? Title,
	string? Message,
	string? CorrelationId = null,
	Guid? Id = null,
	string? TemplateJson = null,
	string? CollapseGroup = null);
