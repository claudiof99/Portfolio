using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface IFestivalService
{
	Task<IReadOnlyList<FestivalDto>> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>Returns only non-hidden festivals. Used exclusively by the public Browse page.</summary>
	Task<IReadOnlyList<FestivalDto>> GetAllVisibleAsync(CancellationToken cancellationToken = default);

	Task<FestivalDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Guid> CreateAsync(FestivalDto festival, CancellationToken cancellationToken = default);
	Task UpdateAsync(FestivalDto festival, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

	/// <summary>Sets the <see cref="FestivalDto.IsHidden"/> flag. Pass <c>true</c> to hide, <c>false</c> to unhide.</summary>
	Task SetHiddenAsync(Guid id, bool isHidden, CancellationToken cancellationToken = default);
}
