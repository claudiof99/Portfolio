using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UmaFestHub.Web.Services;

public interface ITmdbClient
{
    Task<IReadOnlyList<TmdbMovieResult>> SearchMoviesAsync(string query, string? language = null, CancellationToken cancellationToken = default);
    Task<TmdbMovieDetails?> GetMovieDetailsAsync(int tmdbId, string? language = null, CancellationToken cancellationToken = default);
}

public sealed class TmdbMovieResult
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; }
    public string? PosterPath { get; set; }
}

public sealed class TmdbMovieDetails
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int? Runtime { get; set; }
    public IReadOnlyList<string> Genres { get; set; } = new List<string>();
    public double Popularity { get; set; }
}
