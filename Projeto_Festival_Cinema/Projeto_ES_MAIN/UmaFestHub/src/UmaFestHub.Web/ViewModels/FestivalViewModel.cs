namespace UmaFestHub.Web.ViewModels;

public class FestivalViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public bool IsHidden { get; set; }
    public decimal? EarlyBirdDiscountPercent { get; set; }
    public int? EarlyBirdDaysBeforeStart { get; set; }
    public IReadOnlyList<FestivalFilmViewModel> Films { get; set; } = [];
    public decimal? DailyPassPrice { get; set; }
    public decimal? DailyPassDiscountedPrice { get; set; }
    public decimal? CompletePassPrice { get; set; }
    public decimal? CompletePassDiscountedPrice { get; set; }
    public string? CoverImageUrl { get; set; }
    public IReadOnlyList<RecommendationViewModel> Recommendations { get; set; } = [];
}