using System;
using System.Threading;
using System.Threading.Tasks;

namespace UmaFestHub.Application.Interfaces;

public interface IEntitlementService
{
    Task<bool> CanWatchMovieAsync(Guid userId, Guid festivalId, Guid festivalFilmId, Guid? sessionId, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the user has a completed purchase for the given session's ticket, without time restrictions.</summary>
    Task<bool> HasPurchasedForSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default);
}