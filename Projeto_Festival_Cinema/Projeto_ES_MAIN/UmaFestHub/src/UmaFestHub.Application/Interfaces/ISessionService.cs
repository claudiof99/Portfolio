using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Messaging;

namespace UmaFestHub.Application.Interfaces;

public interface ISessionService
{
	Task<IReadOnlyList<SessionDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<SessionDto>> GetByFestivalFilmIdAsync(Guid festivalFilmId, CancellationToken cancellationToken = default);
	Task<(bool Succeeded, Guid? Id, UserMessage? Error)> CreateAsync(SessionDto session, CancellationToken cancellationToken = default);
}
