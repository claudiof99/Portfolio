namespace UmaFestHub.Web.ViewModels;

public class RecommendationViewModel
{
    public Guid FilmId { get; set; }
    public Guid FestivalFilmId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public double Score { get; set; }
}
