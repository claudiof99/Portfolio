namespace UmaFestHub.Web.ViewModels;

public sealed class FestivalFilmViewModel
{
    public Guid Id { get; set; }
    public Guid FilmId { get; set; }

    /// <summary>User already has this <see cref="FilmId"/> in <c>Favorites</c> (see festival detail toolbar).</summary>
    public bool IsFavorite { get; set; }

    /// <summary>User already has this <see cref="FilmId"/> in <c>Watchlist</c>.</summary>
    public bool IsWatchlist { get; set; }
    public string FilmName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? FilmDescription { get; set; }
    public int DurationMinutes { get; set; }
    public IReadOnlyList<string> Genres { get; set; } = [];
    public int SessionCount { get; set; }
    public bool IsWorldPremier { get; set; }
    public string FilmUrl { get; set; } = string.Empty;
    public IReadOnlyList<SessionViewModel> Sessions { get; set; } = [];
}