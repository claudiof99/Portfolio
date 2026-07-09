// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain enum
// Drives nominee source (festival film vs person credit) and INominationValidator choice.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Domain.Enums;

public enum AwardCategory
{
	Film = 0,       // nominates FestivalFilm only
	Actor = 1,      // nominates CreditFilm where Role = "Actor"
	Director = 2,   // nominates CreditFilm where Role = "Director"
	Writing = 3,    // nominates CreditFilm where Role = "Writer"
}
