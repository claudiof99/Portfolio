namespace UmaFestHub.Web.Options;

/// <summary>
/// Binds <c>appsettings.json</c> section <see cref="SectionKey"/> for <see cref="UmaFestHub.Web.Workers.FestivalEndingReminderWorker"/>.
/// The worker runs on a fixed schedule; the “in 3 days” rule itself lives in <c>FestivalEndingReminderService.ReminderWindow</c>.
/// </summary>
public sealed class FestivalEndingReminderOptions
{
	/// <summary>Configuration key under the root JSON object (e.g. <c>"FestivalEndingReminder": { ... }</c>).</summary>
	public const string SectionKey = "FestivalEndingReminder";

	/// <summary>Wait after host start before the first pass; clamped 0–86400 s in the worker.</summary>
	public int InitialDelaySeconds { get; set; } = 60;

	/// <summary>Time between passes; clamped 1–168 h (1 week) in the worker.</summary>
	public int IntervalHours { get; set; } = 6;
}
