namespace UmaFestHub.Domain.Entities;

/// <summary>
/// One membership row: a user has a given <see cref="Film"/> in a typed list (watchlist, favorites, watched).
/// Persisted in table <c>PersonalLists</c> with a unique index on (UserId, Type, FilmId).
/// </summary>
public class PersonalList
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public PersonalListType Type { get; set; }
	public Guid FilmId { get; set; }

	public User? User { get; set; }
	public Film? Film { get; set; }
}
