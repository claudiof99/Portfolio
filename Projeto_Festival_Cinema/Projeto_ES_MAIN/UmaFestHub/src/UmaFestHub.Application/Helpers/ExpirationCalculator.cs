using System;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
 
namespace UmaFestHub.Application.Helpers;

/// <summary>
/// Provides pure functions to calculate if sessions or rentals have expired.
/// </summary>
public static class ExpirationCalculator
{
    /// <summary>
    /// Checks if a rental has expired based on its purchase date and duration.
    /// </summary>
    /// <param name="purchaseDateUtc">The UTC date and time the rental was purchased.</param>
    /// <param name="duration">The duration of the rental.</param>
    /// <param name="nowUtc">The current UTC time to compare against.</param>
    /// <returns>True if the rental is expired, otherwise false.</returns>
    public static bool IsRentalExpired(DateTime purchaseDateUtc, Duration duration, DateTime nowUtc)
    {
        var expiryDate = duration.Unit switch
        {
            DurationUnit.Hours => purchaseDateUtc.AddHours(duration.Value),
            DurationUnit.Days => purchaseDateUtc.AddDays(duration.Value),
            DurationUnit.Minutes => purchaseDateUtc.AddMinutes(duration.Value),
            _ => purchaseDateUtc
        };

        return nowUtc > expiryDate;
    }

    /// <summary>
    /// Checks if a session has expired based on its end time.
    /// </summary>
    /// <param name="sessionEndTimeUtc">The UTC date and time the session ends.</param>
    /// <param name="nowUtc">The current UTC time to compare against.</param>
    /// <returns>True if the session is expired, otherwise false.</returns>
    public static bool IsSessionExpired(DateTime sessionEndTimeUtc, DateTime nowUtc)
    {
        return nowUtc > sessionEndTimeUtc;
    }
}