using System.Globalization;

namespace UmaFestHub.Application.Interfaces;

/// <summary>Culture-aware notification copy (implemented in Web with SharedResources).</summary>
public interface INotificationLocalizer
{
	string Get(string key, params object[] args);
}

/// <summary>Fallback when no culture-aware provider is registered.</summary>
public sealed class NullNotificationLocalizer : INotificationLocalizer
{
	public string Get(string key, params object[] args) => args.Length == 0 ? key : string.Format(CultureInfo.InvariantCulture, key, args);
}
