using UmaFestHub.Domain.ValueObjects;
namespace UmaFestHub.Domain.Entities;

public abstract class Product
{
	public Guid Id { get; set; }
	public decimal Price { get; set; }
	public string ProductType { get; protected set; } = string.Empty;
	
	public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
	public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

	public Product()
	{
		ProductType = GetType().Name;
	}

	public Product(decimal price)
	{
		if (price < 0)
			throw new ArgumentException("Price cannot be negative.");

		Price = price;	
		ProductType = GetType().Name;
	}

	public abstract bool GrantsAccessToSession(Guid sessionId, Guid festivalId, DateTime nowUtc, DateTime sessionStartUtc);

	// Each subclass declares which festival it belongs to.
	// Returns null for products with no festival constraint (e.g. Ticket).
	public abstract Guid? GetFestivalId();
}

public abstract class Pass : Product
{
	public Guid FestivalId { get; set; }
	public Festival? Festival { get; set; }

	protected Pass() { }

	protected Pass(Guid festivalId, decimal price) : base(price)
	{
		if (festivalId == Guid.Empty)
			throw new ArgumentException("Festival cannot be empty.");

		FestivalId = festivalId;
	}

	public override Guid? GetFestivalId() => FestivalId;
}

public sealed class DailyPass : Pass
{
	public DateTime DateUtc { get; set; }

	private DailyPass()
	{
	}

	public DailyPass(Guid festivalId, decimal price, DateTime dateUtc) : base(festivalId, price)
	{
		if (dateUtc == default)
			throw new ArgumentException("Date cannot be empty.");

		DateUtc = dateUtc.ToUniversalTime();
	}

	public override bool GrantsAccessToSession(Guid sessionId, Guid festivalId, DateTime nowUtc, DateTime sessionStartUtc)
		=> FestivalId == festivalId && nowUtc.Date == DateUtc.Date && DateUtc.Date == sessionStartUtc.Date;

}

public sealed class CompletePass : Pass
{
	private CompletePass()
	{
	}

	public CompletePass(Guid festivalId, decimal price) : base(festivalId, price)
	{
		FestivalId = festivalId;
	}
	public override bool GrantsAccessToSession(Guid sessionId, Guid festivalId, DateTime nowUtc, DateTime sessionStartUtc)
		=> FestivalId == festivalId && (Festival == null || nowUtc <= Festival.EndDateUtc);

}

public sealed class Rental : Product
{
	public Guid FestivalFilmId { get; set; }
	public Duration Duration { get; set; } = new();
	public FestivalFilm? FestivalFilm { get; set; }

	public Rental()
	{
		
	}
	public Rental(Guid festivalFilmId, decimal price, Duration duration) : base(price)
	{
		if (festivalFilmId == Guid.Empty)
			throw new ArgumentException("FestivalFilmId cannot be empty.");

		FestivalFilmId = festivalFilmId;
		Duration = duration;
	}

	public override bool GrantsAccessToSession(Guid sessionId, Guid festivalId, DateTime nowUtc, DateTime sessionStartUtc)
		=> false;

	public override Guid? GetFestivalId() => FestivalFilm?.FestivalId;

}

public class Ticket : Product
{

	public Guid SessionId { get; set; }
	public string TicketNumber { get; set; } = string.Empty;
	public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

	public Session? Session { get; set; }

	public Ticket()
	{
		
	}

	public Ticket(Guid sessionId, decimal price) : base(price)
	{
		if (sessionId == Guid.Empty)
			throw new ArgumentException("SessionId cannot be empty.");
		
		SessionId = sessionId;
		TicketNumber = $"TKT-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
		IssuedAtUtc = DateTime.UtcNow;
	}

	public override bool GrantsAccessToSession(Guid sessionId, Guid festivalId, DateTime nowUtc, DateTime sessionStartUtc)
		=> SessionId == sessionId && nowUtc >= sessionStartUtc;

	// Ticket access is scoped to a session, not a festival.
	public override Guid? GetFestivalId() => null;

}