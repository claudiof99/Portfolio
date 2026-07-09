using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

/// <summary>Renders a stored notification template into localized title and message for the current UI culture.</summary>
public interface INotificationTemplateRenderer
{
	(string Title, string Message) Render(NotificationTemplate template);
}
