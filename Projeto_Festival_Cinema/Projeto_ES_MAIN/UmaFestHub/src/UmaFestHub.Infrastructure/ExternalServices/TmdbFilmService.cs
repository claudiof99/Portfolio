using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Infrastructure.ExternalServices;

public class TmdbFilmService : IExternalFilmMetadataService
{
	private readonly IMemoryCache _cache;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConfiguration _configuration;

	public TmdbFilmService(IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		_cache = cache;
		_httpClientFactory = httpClientFactory;
		_configuration = configuration;
	}

	public async Task<ExternalFilmMetadataDto?> GetByExternalIdAsync(int externalId, string? language = null, CancellationToken cancellationToken = default)
	{
		if (externalId <= 0)
		{
			return null;
		}

		var lang = string.IsNullOrWhiteSpace(language) ? "en-US" : language;
		var cacheKey = $"tmdb-film-v5-{externalId}-{lang}";
		if (_cache.TryGetValue(cacheKey, out ExternalFilmMetadataDto? cached))
		{
			return cached;
		}

		var client = _httpClientFactory.CreateClient("TmdbClient");
		var apiKey = ResolveApiKey();
		var token = ResolveAccessToken();

		var movieUrl = BuildMovieUrl(externalId, lang, apiKey);
		var translationsUrl = BuildTranslationsUrl(externalId, apiKey);

		using var movieRequest = CreateRequest(movieUrl, token);
		using var movieResponse = await client.SendAsync(movieRequest, cancellationToken);
		if (!movieResponse.IsSuccessStatusCode)
		{
			return null;
		}

		var json = await movieResponse.Content.ReadAsStringAsync(cancellationToken);
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		var title = root.TryGetProperty("title", out var tProp) ? tProp.GetString() : $"Unknown Film {externalId}";
		var synopsis = root.TryGetProperty("overview", out var oProp) ? oProp.GetString() : "";
		var duration = root.TryGetProperty("runtime", out var rProp) && rProp.ValueKind == JsonValueKind.Number ? rProp.GetInt32() : 0;
		var posterPath = root.TryGetProperty("poster_path", out var pProp) ? pProp.GetString() : null;
		var fullImageUrl = !string.IsNullOrWhiteSpace(posterPath) ? $"https://image.tmdb.org/t/p/w500{posterPath}" : null;
		var popularity = root.TryGetProperty("popularity", out var popProp) && popProp.ValueKind == JsonValueKind.Number ? popProp.GetDouble() : 0.0;

		var genres = MapGenres(root);
		var credits = MapCredits(root);

		if (!IsEnglish(lang))
		{
			using var translationsRequest = CreateRequest(translationsUrl, token);
			using var translationsResponse = await client.SendAsync(translationsRequest, cancellationToken);
			if (translationsResponse.IsSuccessStatusCode)
			{
				var translationsJson = await translationsResponse.Content.ReadAsStringAsync(cancellationToken);
				using var translationsDoc = JsonDocument.Parse(translationsJson);
				if (TryGetTranslation(translationsDoc.RootElement, lang, out var translatedTitle, out var translatedOverview))
				{
					if (!string.IsNullOrWhiteSpace(translatedTitle))
					{
						title = translatedTitle;
					}

					if (!string.IsNullOrWhiteSpace(translatedOverview))
					{
						synopsis = translatedOverview;
					}
				}
			}
		}

		var dto = new ExternalFilmMetadataDto(
			externalId,
			title ?? string.Empty,
			synopsis ?? string.Empty,
			genres,
			duration,
			credits,
			$"https://www.themoviedb.org/movie/{externalId}",
			fullImageUrl,
			popularity);

		_cache.Set(cacheKey, dto, TimeSpan.FromDays(1));
		return dto;
	}

	private static bool TryGetTranslation(
		JsonElement root,
		string language,
		out string? title,
		out string? overview)
	{
		title = null;
		overview = null;

		if (!root.TryGetProperty("translations", out var translations) ||
		    translations.ValueKind != JsonValueKind.Array)
		{
			return false;
		}

		var languagePrefix = language.Split('-')[0];
		JsonElement? preferred = null;
		JsonElement? fallback = null;

		foreach (var item in translations.EnumerateArray())
		{
			if (!item.TryGetProperty("iso_639_1", out var isoProp))
			{
				continue;
			}

			var iso = isoProp.GetString();
			if (!string.Equals(iso, languagePrefix, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			if (!item.TryGetProperty("data", out var data))
			{
				continue;
			}

			var hasOverview = data.TryGetProperty("overview", out var overviewProp) &&
			                  !string.IsNullOrWhiteSpace(overviewProp.GetString());
			if (!hasOverview)
			{
				continue;
			}

			if (item.TryGetProperty("iso_3166_1", out var regionProp) &&
			    string.Equals(regionProp.GetString(), language.Split('-').ElementAtOrDefault(1), StringComparison.OrdinalIgnoreCase))
			{
				preferred = item;
				break;
			}

			fallback ??= item;
		}

		var match = preferred ?? fallback;
		if (match is null)
		{
			return false;
		}

		var matchData = match.Value.GetProperty("data");
		title = matchData.TryGetProperty("title", out var titleProp) ? titleProp.GetString() : null;
		overview = matchData.TryGetProperty("overview", out var overviewProp2) ? overviewProp2.GetString() : null;
		return !string.IsNullOrWhiteSpace(overview);
	}

	private static List<string> MapGenres(JsonElement root)
	{
		var genres = new List<string>();
		if (root.TryGetProperty("genres", out var gProp) && gProp.ValueKind == JsonValueKind.Array)
		{
			foreach (var g in gProp.EnumerateArray())
			{
				var name = g.TryGetProperty("name", out var nProp) ? nProp.GetString() : null;
				if (!string.IsNullOrWhiteSpace(name))
				{
					genres.Add(name);
				}
			}
		}

		return genres;
	}

	private static List<FilmCreditDto> MapCredits(JsonElement root)
	{
		var credits = new List<FilmCreditDto>();
		if (!root.TryGetProperty("credits", out var creditsProp))
		{
			return credits;
		}

		if (creditsProp.TryGetProperty("crew", out var crewProp) && crewProp.ValueKind == JsonValueKind.Array)
		{
			var directors = crewProp.EnumerateArray()
				.Where(c => c.TryGetProperty("job", out var j) && j.GetString() == "Director")
				.Take(2);
			foreach (var d in directors)
			{
				var profilePath = d.TryGetProperty("profile_path", out var p) ? p.GetString() : null;
				var fullProfileUrl = !string.IsNullOrWhiteSpace(profilePath) ? $"https://image.tmdb.org/t/p/w200{profilePath}" : null;
				credits.Add(new FilmCreditDto("Director", d.GetProperty("name").GetString() ?? "Unknown", fullProfileUrl));
			}
		}

		if (creditsProp.TryGetProperty("cast", out var castProp) && castProp.ValueKind == JsonValueKind.Array)
		{
			foreach (var a in castProp.EnumerateArray().Take(5))
			{
				var profilePath = a.TryGetProperty("profile_path", out var p) ? p.GetString() : null;
				var fullProfileUrl = !string.IsNullOrWhiteSpace(profilePath) ? $"https://image.tmdb.org/t/p/w200{profilePath}" : null;
				credits.Add(new FilmCreditDto("Actor", a.GetProperty("name").GetString() ?? "Unknown", fullProfileUrl));
			}
		}

		return credits;
	}

	private string BuildMovieUrl(int externalId, string lang, string apiKey)
	{
		var url = $"https://api.themoviedb.org/3/movie/{externalId}?language={Uri.EscapeDataString(lang)}&append_to_response=credits";
		if (!string.IsNullOrEmpty(apiKey))
		{
			url += $"&api_key={apiKey}";
		}

		return url;
	}

	private static string BuildTranslationsUrl(int externalId, string apiKey)
	{
		var url = $"https://api.themoviedb.org/3/movie/{externalId}/translations";
		if (!string.IsNullOrEmpty(apiKey))
		{
			url += $"?api_key={Uri.EscapeDataString(apiKey)}";
		}

		return url;
	}

	private static HttpRequestMessage CreateRequest(string url, string? token)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		if (!string.IsNullOrEmpty(token))
		{
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
		}

		request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		return request;
	}

	private static bool IsEnglish(string language)
		=> language.StartsWith("en", StringComparison.OrdinalIgnoreCase);

	private string ResolveApiKey()
		=> _configuration["Tmdb:ApiKey"] ?? Environment.GetEnvironmentVariable("TMDB_API_KEY") ?? string.Empty;

	private string? ResolveAccessToken()
		=> _configuration["Tmdb:AccessToken"] ?? Environment.GetEnvironmentVariable("TMDB_TOKEN");
}
