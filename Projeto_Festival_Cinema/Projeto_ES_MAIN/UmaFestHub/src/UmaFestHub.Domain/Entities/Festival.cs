namespace UmaFestHub.Domain.Entities;

public class Festival
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
	public DateTime StartDateUtc { get; set; }
	public DateTime EndDateUtc { get; set; }

	/// <summary>When true the festival is suppressed from public Browse queries. Does not affect entitlements or purchase history.</summary>
	public bool IsHidden { get; set; } = false;

	public decimal? EarlyBirdDiscountPercent { get; set; }
	public int? EarlyBirdDaysBeforeStart { get; set; }

	public ICollection<FestivalFilm> FestivalFilms { get; set; } = new List<FestivalFilm>();
	public ICollection<Award> Awards { get; set; } = new List<Award>();
	public ICollection<Pass> Passes { get; set; } = new List<Pass>();

	public void ConfigureEarlyBirdPromotion(decimal? discountPercent, int? daysBeforeStart)
	{
		if (discountPercent.HasValue)
		{
			if (discountPercent < 0 || discountPercent > 100)
				throw new ArgumentException("Early bird discount must be between 0 and 100.");
			EarlyBirdDiscountPercent = discountPercent;
		}

		if (daysBeforeStart.HasValue)
		{
			if (daysBeforeStart < 1)
				throw new ArgumentException("Early bird days must be at least 1.");
			EarlyBirdDaysBeforeStart = daysBeforeStart;
		}
	}
}
