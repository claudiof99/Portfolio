using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IExternalFilmMetadataService
{
	Task<ExternalFilmMetadataDto?> GetByExternalIdAsync(int externalId, string? language = null, CancellationToken cancellationToken = default);
}

