// In-app notifications: persisted queue row (one per recipient; table "Notifications" in EF).
namespace UmaFestHub.Domain.Entities;

/// <summary>Queued in-app notification (one row per recipient after role fan-out). Dismissed via <see cref="AcknowledgedUtc"/>.</summary>
public class Notification
{
	public Guid Id { get; set; }
	public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;

	/// <summary>JSON template rendered at display time in the user's current culture.</summary>
	public string? TemplateJson { get; set; }

	public string? CorrelationId { get; set; }

	/// <summary>Recipient user (always set — role broadcasts are expanded at enqueue time).</summary>
	public Guid TargetUserId { get; set; }

	/// <summary>Human-readable role context for this row (broadcast role when fan-out, or derived from <see cref="TargetUserId"/> user’s roles for direct messages).</summary>
	public string TargetUserRole { get; set; } = string.Empty;

	/// <summary>When set, this row has been shown and dismissed (OK or modal close).</summary>
	public DateTime? AcknowledgedUtc { get; set; }
}
