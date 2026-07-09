namespace UmaFestHub.Application.Recommendations;

public interface IRecommendationService
{
    Task<IReadOnlyList<FilmRecommendationDto>> GetAsync(
        Guid userId, Guid festivalId, int maxResults = 6, CancellationToken cancellationToken = default);

    /// <summary>Get recommendations from all festivals the user has access to.</summary>
    Task<IReadOnlyList<FilmRecommendationDto>> GetFromAllFestivalsAsync(
        Guid userId, int maxResults = 6, CancellationToken cancellationToken = default);
}
