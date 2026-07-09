using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// Thin application service over <see cref="IPersonalListRepository"/>: forwards add/remove/list without extra business rules.
/// </summary>
public class PersonalListService : IPersonalListService
{
	private readonly IPersonalListRepository _personalListRepository;

	public PersonalListService(IPersonalListRepository personalListRepository)
	{
		_personalListRepository = personalListRepository;
	}

	public async Task AddFilmAsync(Guid userId, PersonalListType type, Guid filmId, CancellationToken cancellationToken = default)
	{
		await _personalListRepository.AddAsync(userId, filmId, type, cancellationToken);
	}

	public async Task RemoveFilmAsync(Guid userId, PersonalListType type, Guid filmId, CancellationToken cancellationToken = default)
	{
		await _personalListRepository.RemoveAsync(userId, filmId, type, cancellationToken);
	}

	public async Task<IReadOnlyList<Guid>> GetListAsync(Guid userId, PersonalListType type, CancellationToken cancellationToken = default)
	{
		return await _personalListRepository.GetByUserAndTypeAsync(userId, type, cancellationToken);
	}
}
