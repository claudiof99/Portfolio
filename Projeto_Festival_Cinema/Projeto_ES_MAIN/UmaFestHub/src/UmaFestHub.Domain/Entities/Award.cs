// -----------------------------------------------------------------------------
// Awards, nominations & votes — Domain
// Award aggregate root + AwardNomination: static factories enforce creation rules
// (name, festival, four unique nominees); Film vs credit nominations by category.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.ValueObjects;


public class Award
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public Guid FestivalId { get; set; }
	public AwardCategory Category { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime EndDateUtc { get; set; }
	public bool IsActive { get; set; } = true;

	public Festival? Festival { get; set; }
	public ICollection<AwardNomination> Nominations { get; set; } = new List<AwardNomination>();

	public static Award Create(
		Guid festivalId,
		string awardName,
		AwardCategory category,
		DateTime createdAtUtc,
		DateTime endDateUtc)
	{
		if (festivalId == Guid.Empty)
		{
			throw new ArgumentException("FestivalId is required.", nameof(festivalId));
		}

		if (string.IsNullOrWhiteSpace(awardName))
		{
			throw new ArgumentException("Award name is required.", nameof(awardName));
		}

		ValidateEndDateUtc(endDateUtc, createdAtUtc);

		return new Award
		{
			Id = Guid.NewGuid(),
			FestivalId = festivalId,
			Name = awardName.Trim(),
			Category = category,
			CreatedAtUtc = createdAtUtc,
			EndDateUtc = endDateUtc,
			IsActive = true
		};
	}

	public static (Award Award, IReadOnlyList<AwardNomination> Nominations) CreateWithNominees(
		Guid festivalId,
		string awardName,
		AwardCategory category,
		IReadOnlyList<Guid> nomineeIds,
		DateTime createdAtUtc,
		DateTime endDateUtc)
	{
		if (festivalId == Guid.Empty)
		{
			throw new ArgumentException("FestivalId is required.", nameof(festivalId));
		}

		if (string.IsNullOrWhiteSpace(awardName))
		{
			throw new ArgumentException("Award name is required.", nameof(awardName));
		}

		if (nomineeIds is null || nomineeIds.Count != 4)
		{
			throw new ArgumentException("Exactly 4 nominees are required.", nameof(nomineeIds));
		}

		if (nomineeIds.Any(x => x == Guid.Empty))
		{
			throw new ArgumentException("Nominee IDs cannot be empty.", nameof(nomineeIds));
		}

		if (nomineeIds.Distinct().Count() != nomineeIds.Count)
		{
			throw new ArgumentException("Nominees must be unique.", nameof(nomineeIds));
		}

		ValidateEndDateUtc(endDateUtc, createdAtUtc);

		var award = new Award
		{
			Id = Guid.NewGuid(),
			FestivalId = festivalId,
			Name = awardName.Trim(),
			Category = category,
			CreatedAtUtc = createdAtUtc,
			EndDateUtc = endDateUtc,
			IsActive = true
		};

		var nominations = nomineeIds.Select(nomineeId => new AwardNomination
		{
			Id = Guid.NewGuid(),
			AwardId = award.Id,
			FestivalFilmId = category == AwardCategory.Film ? nomineeId : null,
			CreditFilmId = category == AwardCategory.Film ? null : nomineeId
		}).ToList();

		return (award, nominations);
	}

	private static void ValidateEndDateUtc(DateTime endDateUtc, DateTime createdAtUtc)
	{
		if (endDateUtc <= createdAtUtc)
		{
			throw new ArgumentException("End date must be after the award creation time.", nameof(endDateUtc));
		}
	}
}

public class AwardNomination
{
	public Guid Id { get; set; }
	public Guid AwardId { get; set; }
	public Guid? FestivalFilmId { get; set; }
	public Guid? CreditFilmId { get; set; }

	public Award? Award { get; set; }
	public FestivalFilm? FestivalFilm { get; set; }
	public CreditFilm? CreditFilm { get; set; }
	public ICollection<Vote> Votes { get; set; } = new List<Vote>();

	public static AwardNomination CreateFilmNomination(Guid awardId, Guid festivalFilmId)
	{
		if (awardId == Guid.Empty)
		{
			throw new ArgumentException("AwardId is required.", nameof(awardId));
		}

		if (festivalFilmId == Guid.Empty)
		{
			throw new ArgumentException("FestivalFilmId is required.", nameof(festivalFilmId));
		}

		return new AwardNomination
		{
			Id = Guid.NewGuid(),
			AwardId = awardId,
			FestivalFilmId = festivalFilmId,
			CreditFilmId = null
		};
	}

	public static AwardNomination CreateCreditNomination(Guid awardId, Guid creditFilmId)
	{
		if (awardId == Guid.Empty)
		{
			throw new ArgumentException("AwardId is required.", nameof(awardId));
		}

		if (creditFilmId == Guid.Empty)
		{
			throw new ArgumentException("CreditFilmId is required.", nameof(creditFilmId));
		}

		return new AwardNomination
		{
			Id = Guid.NewGuid(),
			AwardId = awardId,
			FestivalFilmId = null,
			CreditFilmId = creditFilmId
		};
	}
}
