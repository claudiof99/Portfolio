using UmaFestHub.Domain.ValueObjects;

namespace UmaFestHub.Domain.Interfaces;

public interface ICreditRepository
{
	Task<IReadOnlyList<CreditFilm>> GetByFestivalAndRoleAsync(Guid festivalId, string role, CancellationToken cancellationToken = default);
}

