namespace UmaFestHub.Application.Recommendations;

public sealed class RecommendationContext
{
    public required Guid UserId { get; init; }
    public required Guid FestivalId { get; init; }
    public required IReadOnlyList<Domain.Entities.FestivalFilm> FestivalFilms { get; init; }
}
