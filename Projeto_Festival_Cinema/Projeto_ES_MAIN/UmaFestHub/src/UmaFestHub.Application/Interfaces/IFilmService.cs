using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IFilmService
{
	Task<IReadOnlyList<FilmDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<FilmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<FilmDto?> GetByIdLocalizedAsync(Guid id, string? tmdbLanguage, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<FilmDto>> LocalizeFilmsAsync(IReadOnlyList<FilmDto> films, string? tmdbLanguage, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<FilmDto>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<FilmDto>> SearchAsync(string? title, string? genre, int? minDurationMinutes, int? maxDurationMinutes, CancellationToken cancellationToken = default);
	Task<Guid> CreateAsync(FilmDto film, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
