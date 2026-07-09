// -----------------------------------------------------------------------------
// Awards, nominations & votes — Application
// Supplies dropdown options for the four nominee slots (festival films vs credits).
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

public sealed class NominationCandidatesService : INominationCandidatesService
{
	private readonly IFestivalFilmRepository _festivalFilmRepository;
	private readonly ICreditRepository _creditRepository;

	public NominationCandidatesService(IFestivalFilmRepository festivalFilmRepository, ICreditRepository creditRepository)
	{
		_festivalFilmRepository = festivalFilmRepository;
		_creditRepository = creditRepository;
	}

	public async Task<IReadOnlyList<NomineeOptionDto>> GetCandidatesAsync(AwardCategory category, Guid festivalId, CancellationToken ct = default)
	{
		return category switch
		{
			AwardCategory.Film => (await _festivalFilmRepository.GetByFestivalIdAsync(festivalId, ct))
				.Select(ff => new NomineeOptionDto(
					ff.Id,
					ff.Film?.Name ?? ff.Film?.Title ?? "Unknown film",
					ff.Film?.ImageUrl))
				.OrderBy(x => x.Label)
				.ToList(),

			AwardCategory.Actor => await GetCreditsAsync(festivalId, "Actor", ct),
			AwardCategory.Director => await GetCreditsAsync(festivalId, "Director", ct),
			AwardCategory.Writing => await GetCreditsAsync(festivalId, "Writer", ct),

			_ => Array.Empty<NomineeOptionDto>()
		};
	}

	private async Task<IReadOnlyList<NomineeOptionDto>> GetCreditsAsync(Guid festivalId, string role, CancellationToken ct)
	{
		var credits = await _creditRepository.GetByFestivalAndRoleAsync(festivalId, role, ct);

		// Deduplicate by PersonId so the same person doesn't appear multiple times for different films.
		return credits
			.Where(c => c.PersonId != Guid.Empty && c.Person != null)
			.GroupBy(c => c.PersonId)
			.Select(g => g.First())
			.Select(c => new NomineeOptionDto(c.Id, c.Person!.Name, c.Person!.ImageUrl))
			.OrderBy(x => x.Label)
			.ToList();
	}
}
