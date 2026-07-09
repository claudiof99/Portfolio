namespace UmaFestHub.Domain.Entities;

public class FestivalFilm
{
	public Guid Id { get; set; }
	public Guid FestivalId { get; set; }
	public Guid FilmId { get; set; }
	public bool IsWorldPremier { get; set; }
	public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
	public string ProgrammingNotes { get; set; } = string.Empty;// Notes for festival programmers, not visible to users

	public Festival? Festival { get; set; }
	public Film? Film { get; set; }
	public ICollection<Session> Sessions { get; set; } = new List<Session>();
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
	public ICollection<AwardNomination> AwardNomination { get; set; } = new List<AwardNomination>();
	public ICollection<Rental> Rentals { get; set; } = new List<Rental>();


	
}
