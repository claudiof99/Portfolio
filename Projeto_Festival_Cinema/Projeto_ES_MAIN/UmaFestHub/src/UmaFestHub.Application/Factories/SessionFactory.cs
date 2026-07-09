using System;
using System.Collections.Generic;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Factories;

public abstract class SessionStore
{
    public abstract string SessionType { get; }

    public abstract Session Create(
        Guid festivalFilmId,
        DateTime startTimeUtc,
        DateTime endTimeUtc);
}

public class FixedSessionStore : SessionStore
{
    public override string SessionType => nameof(FixedSession);

    public override Session Create(
        Guid festivalFilmId,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
        => new FixedSession(festivalFilmId, startTimeUtc, endTimeUtc);
}

public class PremierSessionStore : SessionStore
{
    public override string SessionType => nameof(PremierSession);

    public override Session Create(
        Guid festivalFilmId,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
        => new PremierSession(festivalFilmId, startTimeUtc, endTimeUtc);
}

public class AccessWindowSessionStore : SessionStore
{
    public override string SessionType => nameof(AccessWindowSession);

    public override Session Create(
        Guid festivalFilmId,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
    {
        // if (startTimeUtc is null || endTimeUtc is null)
        //     throw new ArgumentException("AccessStartUtc and AccessEndUtc are required for AccessWindowSession.");

        return new AccessWindowSession(festivalFilmId, startTimeUtc, endTimeUtc);
    }
}
