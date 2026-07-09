namespace UmaFestHub.Web.Options;

/// <summary>
/// Binds <c>appsettings.json</c> section <see cref="SectionKey"/> for <see cref="UmaFestHub.Web.Workers.RentalExpiryReminderWorker"/>.
/// The 3-day expiry horizon is fixed in <see cref="UmaFestHub.Application.Services.RentalExpiryReminderService"/> (same pattern as festival-ending reminders).
/// </summary>
public sealed class RentalExpiryReminderOptions
{
	public const string SectionKey = "RentalExpiryReminder";

	/// <summary>Delay before the first rental-expiry pass after the host starts.</summary>
	public int InitialDelaySeconds { get; set; } = 90;

	/// <summary>Sleep between passes that call <see cref="UmaFestHub.Application.Interfaces.IRentalExpiryReminderService.EnqueueRentalExpiryRemindersAsync"/>.</summary>
	public int IntervalHours { get; set; } = 6;
}
