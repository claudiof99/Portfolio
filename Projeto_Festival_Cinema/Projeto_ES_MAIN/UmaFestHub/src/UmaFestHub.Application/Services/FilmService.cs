using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Application.Constants;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to manage our film catalog, handle TMDb data mapping, and prevent duplicate entries.
/// </summary>
public class FilmService : IFilmService
{
	private readonly IFilmRepository _filmRepository;
	private readonly IExternalFilmMetadataService _externalFilmMetadataService;
	private readonly ILogger<FilmService> _logger;

	public FilmService(
		IFilmRepository filmRepository,
		IExternalFilmMetadataService externalFilmMetadataService,
		ILogger<FilmService> logger)
	{
		_filmRepository = filmRepository;
		_externalFilmMetadataService = externalFilmMetadataService;
		_logger = logger;
	}

	/// <summary>
	/// We fetch the entire catalog of films currently stored in our database.
	/// </summary>
	public async Task<IReadOnlyList<FilmDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var films = await _filmRepository.GetAllAsync(cancellationToken);
		return films.Select(Map).ToList();
	}

	/// <summary>
	/// We retrieve a single film's details based on its ID.
	/// </summary>
	public async Task<FilmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var film = await _filmRepository.GetByIdAsync(id, cancellationToken);
		return film is null ? null : Map(film);
	}

	public async Task<FilmDto?> GetByIdLocalizedAsync(Guid id, string? tmdbLanguage, CancellationToken cancellationToken = default)
	{
		var film = await GetByIdAsync(id, cancellationToken);
		if (film is null)
		{
			return null;
		}

		return await LocalizeFilmDtoAsync(film, tmdbLanguage, cancellationToken);
	}

	public async Task<IReadOnlyList<FilmDto>> LocalizeFilmsAsync(
		IReadOnlyList<FilmDto> films,
		string? tmdbLanguage,
		CancellationToken cancellationToken = default)
	{
		if (films.Count == 0 || string.IsNullOrWhiteSpace(tmdbLanguage))
		{
			return films;
		}

		var tasks = films.Select(f => LocalizeFilmDtoAsync(f, tmdbLanguage, cancellationToken));
		return await Task.WhenAll(tasks);
	}

	public async Task<IReadOnlyList<FilmDto>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
	{
		var films = await _filmRepository.GetByIdsAsync(ids, cancellationToken);
		return films.Select(Map).ToList();
	}

	/// <summary>
	/// We search our film catalog using various optional filters like title, genre, and duration.
	/// </summary>
	public async Task<IReadOnlyList<FilmDto>> SearchAsync(string? title, string? genre, int? minDurationMinutes, int? maxDurationMinutes, CancellationToken cancellationToken = default)
	{
		var films = await _filmRepository.GetAllAsync(cancellationToken);
		var query = films.AsEnumerable();

		if (!string.IsNullOrWhiteSpace(title))
		{
			// We update the main text search to look for matches in BOTH the film's Name AND its Genres.
			query = query.Where(x => x.Name.Contains(title, StringComparison.OrdinalIgnoreCase) || x.Genres.Any(g => g.Name.Contains(title, StringComparison.OrdinalIgnoreCase)));
		}

		if (!string.IsNullOrWhiteSpace(genre))
		{
			// Using Contains instead of Equals makes the genre dropdown filter more resilient to formatting differences.
			query = query.Where(x => x.Genres.Any(g => g.Name.Contains(genre, StringComparison.OrdinalIgnoreCase)));
		}

		if (minDurationMinutes.HasValue)
		{
			query = query.Where(x => x.Duration.ToMinutes() >= minDurationMinutes.Value);
		}

		if (maxDurationMinutes.HasValue)
		{
			query = query.Where(x => x.Duration.ToMinutes() <= maxDurationMinutes.Value);
		}

		return query.Select(Map).ToList();
	}

/// <summary>
	/// We create a new film. If an external TMDb ID is provided, we check for duplicates first, then fetch and map the metadata automatically.
	/// </summary>
	public async Task<Guid> CreateAsync(FilmDto film, CancellationToken cancellationToken = default)
	{
		ExternalFilmMetadataDto? metadata = null;
		if (film.ExternalId > 0)
		{
			// Deduplication check
			var existing = await _filmRepository.GetByExternalIdAsync(film.ExternalId, cancellationToken);
			if (existing != null)
			{
				_logger.LogInformation("Film with external id {ExternalId} already exists: {FilmId}", film.ExternalId, existing.Id);
				return existing.Id;
			}
			metadata = await _externalFilmMetadataService.GetByExternalIdAsync(film.ExternalId, null, cancellationToken);
		}

		var entity = new Film
		{
			Id = film.Id == Guid.Empty ? Guid.NewGuid() : film.Id,
			ExternalId = film.ExternalId,
			Name = metadata?.Title ?? film.Name,
			Url = metadata?.ViewingUrl ?? film.Url,
			ImageUrl = metadata?.PosterUrl,
			Description = metadata?.Synopsis ?? film.Description,
			Duration = new Duration
			{
				Value = metadata?.DurationMinutes ?? film.DurationMinutes,
				Unit = DurationUnit.Minutes
			},
			Genres = (metadata?.Genres ?? film.Genres)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Select(name => new Genre { Id = Guid.NewGuid(), Name = name })
				.ToList(),
			TmdbPopularity = (decimal)(metadata?.Popularity ?? 0.0),
			Credits = (metadata?.Credits ?? [])
				.Select(c => new CreditFilm
				{
					Id = Guid.NewGuid(),
					Role = c.Role,
					Person = new Person { Id = Guid.NewGuid(), Name = c.PersonName, ImageUrl = c.ImageUrl }
				}).ToList()
		};

		await _filmRepository.AddAsync(entity, cancellationToken);
		_logger.LogInformation("Film {FilmName} created with external id {ExternalId}", entity.Name, entity.ExternalId);
		return entity.Id;
	}

	/// <summary>
	/// We map the Film entity to a FilmDto, flattening the relational lists (like genres) for the UI.
	/// </summary>
	private static FilmDto Map(Film film) =>
		new(
			film.Id,
			film.ExternalId,
			film.Name,
			film.Url,
			film.ImageUrl,
			film.Description,
			film.Duration.ToMinutes(),
			film.Genres.Select(x => x.Name).ToList(),
			film.Credits?
				.Select(c => new FilmCreditDto(c.Role ?? "Unknown", c.Person?.Name ?? "Unknown", c.Person?.ImageUrl))
				.ToList() 
				?? new List<FilmCreditDto>());

	/// <summary>
	/// We permanently delete a film from our catalog.
	/// </summary>
	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		await _filmRepository.DeleteAsync(id, cancellationToken);
	}

	private async Task<FilmDto> LocalizeFilmDtoAsync(
		FilmDto film,
		string? tmdbLanguage,
		CancellationToken cancellationToken)
	{
		if (film.ExternalId <= 0 || string.IsNullOrWhiteSpace(tmdbLanguage) || film.Id == SeedContentIds.MidnightFramesFilmId)
		{
			return film;
		}

		var metadata = await _externalFilmMetadataService.GetByExternalIdAsync(
			film.ExternalId,
			tmdbLanguage,
			cancellationToken);
		if (metadata is null)
		{
			return film;
		}

		return film with
		{
			Name = string.IsNullOrWhiteSpace(metadata.Title) ? film.Name : metadata.Title,
			Description = string.IsNullOrWhiteSpace(metadata.Synopsis) ? film.Description : metadata.Synopsis,
			Genres = metadata.Genres.Count > 0 ? metadata.Genres : film.Genres,
			Credits = metadata.Credits.Count > 0 ? metadata.Credits : film.Credits,
		};
	}
}
