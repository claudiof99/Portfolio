/*
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Services;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace UmaFestHub.Application.Tests;

public class FilmServiceTests
{
	[Fact]
	public async Task CreateAsync_ShouldPersistFilm()
	{
		var repository = new InMemoryFilmRepository();
		var service = new FilmService(repository, new DummyExternalFilmService(), NullLogger<FilmService>.Instance);

		var id = await service.CreateAsync(new FilmDto(Guid.Empty, 42, "Demo", "url", null, "desc", 120, new[] { "Drama" }));
		var created = await service.GetByIdAsync(id);

		Assert.NotNull(created);
		Assert.Equal("Demo", created!.Name);
	}

	private sealed class DummyExternalFilmService : IExternalFilmMetadataService
	{
		public Task<ExternalFilmMetadataDto?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
			=> Task.FromResult<ExternalFilmMetadataDto?>(null);
	}

	private sealed class InMemoryFilmRepository : IFilmRepository
	{
		private readonly List<Film> _items = new();

		public Task<IReadOnlyList<Film>> GetAllAsync(CancellationToken cancellationToken = default)
			=> Task.FromResult<IReadOnlyList<Film>>(_items);

		public Task<Film?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
			=> Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

		public Task AddAsync(Film film, CancellationToken cancellationToken = default)
		{
			_items.Add(film);
			return Task.CompletedTask;
		}

		public Task<Film?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
			=> Task.FromResult(_items.FirstOrDefault(x => x.ExternalId == externalId));
	}
}
*/