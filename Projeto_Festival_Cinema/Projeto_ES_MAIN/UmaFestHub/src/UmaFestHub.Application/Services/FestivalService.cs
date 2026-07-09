using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Factories;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to handle the core CRUD (Create, Read, Update, Delete) operations for our festivals.
/// </summary>
public class FestivalService : IFestivalService
{
	private readonly IFestivalRepository _festivalRepository;

	public FestivalService(IFestivalRepository festivalRepository)
	{
		_festivalRepository = festivalRepository;
	}

	/// <summary>
	/// We retrieve a list of all festivals available on the platform.
	/// </summary>
	public async Task<IReadOnlyList<FestivalDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var festivals = await _festivalRepository.GetAllAsync(cancellationToken);
		return festivals.Select(Map).ToList();
	}

	/// <summary>
	/// Returns only non-hidden festivals. Used exclusively by the public Browse page.
	/// </summary>
	public async Task<IReadOnlyList<FestivalDto>> GetAllVisibleAsync(CancellationToken cancellationToken = default)
	{
		var festivals = await _festivalRepository.GetAllVisibleAsync(cancellationToken);
		return festivals.Select(Map).ToList();
	}

	/// <summary>
	/// We fetch a specific festival by its unique identifier.
	/// </summary>
	public async Task<FestivalDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var festival = await _festivalRepository.GetByIdAsync(id, cancellationToken);
		return festival is null ? null : Map(festival);
	}

	/// <summary>
	/// We create a new festival entry in our system and return its generated ID.
	/// </summary>
	public async Task<Guid> CreateAsync(FestivalDto festival, CancellationToken cancellationToken = default)
	{
		if (festival.EndDateUtc <= festival.StartDateUtc)
		{
			throw new ArgumentException("Festival_EndDateBeforeStart");
		}

		var entity = new Festival
		{
			Id = festival.Id == Guid.Empty ? Guid.NewGuid() : festival.Id,
			Name = festival.Name,
			Description = festival.Description,
			StartDateUtc = festival.StartDateUtc,
			EndDateUtc = festival.EndDateUtc
		};

		if (festival.EarlyBirdDiscountPercent.HasValue || festival.EarlyBirdDaysBeforeStart.HasValue)
		{
			entity.ConfigureEarlyBirdPromotion(festival.EarlyBirdDiscountPercent, festival.EarlyBirdDaysBeforeStart);
		}

		await _festivalRepository.AddAsync(entity, cancellationToken);

		return entity.Id;
	}

	/// <summary>
	/// We update an existing festival's details, ensuring it exists before making changes.
	/// </summary>
	public async Task UpdateAsync(FestivalDto festival, CancellationToken cancellationToken = default)
	{
		if (festival.EndDateUtc <= festival.StartDateUtc)
		{
			throw new ArgumentException("Festival_EndDateBeforeStart");
		}

		var existing = await _festivalRepository.GetByIdAsync(festival.Id, cancellationToken);
		if (existing is null)
		{
			throw new KeyNotFoundException($"Festival with id '{festival.Id}' was not found.");
		}

		existing.Name = festival.Name;
		existing.Description = festival.Description;
		existing.StartDateUtc = festival.StartDateUtc;
		existing.EndDateUtc = festival.EndDateUtc;

		if (festival.EarlyBirdDiscountPercent.HasValue || festival.EarlyBirdDaysBeforeStart.HasValue)
		{
			existing.ConfigureEarlyBirdPromotion(festival.EarlyBirdDiscountPercent, festival.EarlyBirdDaysBeforeStart);
		}
		else
		{
			existing.EarlyBirdDiscountPercent = null;
			existing.EarlyBirdDaysBeforeStart = null;
		}

		await _festivalRepository.UpdateAsync(existing, cancellationToken);
	}

	/// <summary>
	/// We permanently delete a festival from the platform.
	/// </summary>
	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		await _festivalRepository.DeleteAsync(id, cancellationToken);
	}

	/// <summary>
	/// Sets or clears the IsHidden flag on a festival. Persists via UpdateAsync so no purchase/entitlement data is touched.
	/// </summary>
	public async Task SetHiddenAsync(Guid id, bool isHidden, CancellationToken cancellationToken = default)
	{
		var existing = await _festivalRepository.GetByIdAsync(id, cancellationToken);
		if (existing is null)
			throw new KeyNotFoundException($"Festival with id '{id}' was not found.");

		existing.IsHidden = isHidden;
		await _festivalRepository.UpdateAsync(existing, cancellationToken);
	}

	/// <summary>
	/// We map the database entity to a DTO for safe data transfer to the presentation layer.
	/// </summary>
	private static FestivalDto Map(Festival festival) =>
		new(festival.Id, festival.Name, festival.Description, festival.StartDateUtc, festival.EndDateUtc,
			festival.EarlyBirdDiscountPercent, festival.EarlyBirdDaysBeforeStart, festival.IsHidden);
}
