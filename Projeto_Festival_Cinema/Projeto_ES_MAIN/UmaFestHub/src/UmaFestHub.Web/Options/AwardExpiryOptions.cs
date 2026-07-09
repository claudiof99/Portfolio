namespace UmaFestHub.Web.Options;

/// <summary>
/// Binds <c>appsettings.json</c> section <see cref="SectionKey"/> for <see cref="Workers.AwardExpiryWorker"/>.
/// </summary>
public sealed class AwardExpiryOptions
{
	public const string SectionKey = "AwardExpiry";

	public int InitialDelaySeconds { get; set; } = 60;

	public int IntervalMinutes { get; set; } = 60;
}
