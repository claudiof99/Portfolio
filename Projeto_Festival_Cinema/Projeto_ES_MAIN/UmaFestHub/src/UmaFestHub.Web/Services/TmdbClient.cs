using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace UmaFestHub.Web.Services;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public TmdbClient(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<TmdbMovieResult>> SearchMoviesAsync(string query, string? language = null, CancellationToken cancellationToken = default)
    {
        var apiKey = ResolveApiKey();
        var lang = string.IsNullOrWhiteSpace(language) ? "en-US" : language;
        var url = $"search/movie?api_key={Uri.EscapeDataString(apiKey)}&language={Uri.EscapeDataString(lang)}&query={Uri.EscapeDataString(query)}";
        var res = await _http.GetAsync(url, cancellationToken);
        res.EnsureSuccessStatusCode();
        using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = doc.RootElement;
        var list = new List<TmdbMovieResult>();
        if (root.TryGetProperty("results", out var results))
        {
            foreach (var item in results.EnumerateArray())
            {
                var m = new TmdbMovieResult
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Title = item.GetProperty("title").GetString() ?? string.Empty,
                    Overview = item.GetProperty("overview").GetString() ?? string.Empty,
                    ReleaseDate = item.TryGetProperty("release_date", out var rd) ? rd.GetString() : null,
                    PosterPath = item.TryGetProperty("poster_path", out var pp) ? pp.GetString() : null
                };
                list.Add(m);
            }
        }

        return list;
    }

    public async Task<TmdbMovieDetails?> GetMovieDetailsAsync(int tmdbId, string? language = null, CancellationToken cancellationToken = default)
    {
        var apiKey = ResolveApiKey();
        var lang = string.IsNullOrWhiteSpace(language) ? "en-US" : language;
        var url = $"movie/{tmdbId}?api_key={Uri.EscapeDataString(apiKey)}&language={Uri.EscapeDataString(lang)}&append_to_response=credits";
        var res = await _http.GetAsync(url, cancellationToken);
        if (!res.IsSuccessStatusCode) return null;
        using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = doc.RootElement;
        var details = new TmdbMovieDetails
        {
            Id = root.GetProperty("id").GetInt32(),
            Title = root.GetProperty("title").GetString() ?? string.Empty,
            Overview = root.GetProperty("overview").GetString() ?? string.Empty,
            Runtime = root.TryGetProperty("runtime", out var rt) && rt.ValueKind != JsonValueKind.Null ? rt.GetInt32() : null,
            Genres = MapGenres(root),
            Popularity = root.TryGetProperty("popularity", out var pop)
                && pop.ValueKind == JsonValueKind.Number
                ? pop.GetDouble()
                : 0.0
        };

        return details;
    }

    private static IReadOnlyList<string> MapGenres(JsonElement root)
    {
        var list = new List<string>();
        if (root.TryGetProperty("genres", out var genres))
        {
            foreach (var g in genres.EnumerateArray())
            {
                if (g.TryGetProperty("name", out var name)) list.Add(name.GetString() ?? string.Empty);
            }
        }
        return list;
    }

    private string ResolveApiKey()
    {
        var apiKey = _configuration["TMDB_API_KEY"]
                     ?? _configuration["TMDb:ApiKey"]
                     ?? Environment.GetEnvironmentVariable("TMDB_API_KEY")
                     ?? string.Empty;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Festival_TmdbApiKeyNotConfigured");
        }

        return apiKey;
    }
}
