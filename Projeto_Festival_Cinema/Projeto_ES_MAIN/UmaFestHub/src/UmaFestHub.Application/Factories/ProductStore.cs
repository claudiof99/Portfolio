using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;

namespace UmaFestHub.Application.Factories;

public abstract class ProductStore
{
    public abstract string ProductType { get; }

    public abstract Product Create(
        decimal price,
        Guid? sessionId = null,
        Guid? festivalId = null,
        Guid? festivalFilmId = null,
        DateTime? dateUtc = null,
        Duration? duration = null);
}

public sealed class TicketStore : ProductStore
{
    public override string ProductType => nameof(Ticket);

    public override Product Create(
        decimal price,
        Guid? sessionId = null,
        Guid? festivalId = null,
        Guid? festivalFilmId = null,
        DateTime? dateUtc = null,
        Duration? duration = null)
    {
        if (!sessionId.HasValue || sessionId.Value == Guid.Empty)
            throw new ArgumentException("SessionId is required for Ticket.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        return new Ticket(sessionId.Value, price);
    }
}

public sealed class DailyPassStore : ProductStore
{
    public override string ProductType => nameof(DailyPass);

    public override Product Create(
        decimal price,
        Guid? sessionId = null,
        Guid? festivalId = null,
        Guid? festivalFilmId = null,
        DateTime? dateUtc = null,
        Duration? duration = null)
    {
        if (!festivalId.HasValue || festivalId.Value == Guid.Empty)
            throw new ArgumentException("FestivalId is required for DailyPass.");

        if (!dateUtc.HasValue)
            throw new ArgumentException("DateUtc is required for DailyPass.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        return new DailyPass(festivalId.Value, price, dateUtc.Value);
    }
}

public sealed class CompletePassStore : ProductStore
{
    public override string ProductType => nameof(CompletePass);

    public override Product Create(
        decimal price,
        Guid? sessionId = null,
        Guid? festivalId = null,
        Guid? festivalFilmId = null,
        DateTime? dateUtc = null,
        Duration? duration = null)
    {
        if (!festivalId.HasValue || festivalId.Value == Guid.Empty)
            throw new ArgumentException("FestivalId is required for CompletePass.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        return new CompletePass(festivalId.Value, price);
    }
}

public sealed class RentalStore : ProductStore
{
    public override string ProductType => nameof(Rental);

    public override Product Create(
        decimal price,
        Guid? sessionId = null,
        Guid? festivalId = null,
        Guid? festivalFilmId = null,
        DateTime? dateUtc = null,
        Duration? duration = null)
    {
        if (!festivalFilmId.HasValue || festivalFilmId.Value == Guid.Empty)
            throw new ArgumentException("FestivalFilmId is required for Rental.");

        if (duration is null)
            throw new ArgumentException("Duration is required for Rental.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        return new Rental(festivalFilmId.Value, price, duration);
    }
}
