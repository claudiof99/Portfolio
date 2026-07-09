namespace UmaFestHub.Domain.Entities;

public abstract class Session
{
	public Guid Id { get; set; }
	public Guid FestivalFilmId { get; set; }
	public DateTime StartTimeUtc { get; set; }
	public DateTime EndTimeUtc { get; set; }
	public string SessionType { get; protected set; } = string.Empty;
	public FestivalFilm? FestivalFilm { get; set; }
	public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

	protected Session(Guid festivalFilmId, DateTime startTimeUtc, DateTime endTimeUtc)
	{
		if (festivalFilmId == Guid.Empty)
			throw new ArgumentException("FestivalFilmId cannot be empty.");
		if (endTimeUtc <= startTimeUtc)
			throw new ArgumentException("EndTimeUtc must be after StartTimeUtc.");
		
		FestivalFilmId = festivalFilmId;
		StartTimeUtc = startTimeUtc;
		EndTimeUtc = endTimeUtc;
	}

}

public sealed class PremierSession : Session
{
	public PremierSession(Guid festivalFilmId, DateTime startTimeUtc, DateTime endTimeUtc) : base(festivalFilmId, startTimeUtc, endTimeUtc)
	{
		SessionType = nameof(PremierSession);
	}
}

public sealed class FixedSession : Session
{
	public FixedSession(Guid festivalFilmId, DateTime startTimeUtc, DateTime endTimeUtc) : base(festivalFilmId, startTimeUtc, endTimeUtc)
	{
		SessionType = nameof(FixedSession);
	}
}

public sealed class AccessWindowSession : Session
{
	/*
	public DateTime AccessStartUtc { get; set; }
	public DateTime AccessEndUtc { get; set; }
	*/
	public AccessWindowSession(Guid festivalFilmId, DateTime startTimeUtc, DateTime endTimeUtc) : base(festivalFilmId, startTimeUtc, endTimeUtc)
	{
		// AccessStartUtc = accessStartUtc;
		// AccessEndUtc = accessEndUtc;
		
		SessionType = nameof(AccessWindowSession);
	}

	
}