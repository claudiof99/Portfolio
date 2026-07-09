using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Messaging;

namespace UmaFestHub.Application.Interfaces;

public interface ISessionAccessService
{
	Task<(bool Allowed, UserMessage? Error)> ValidateAccessAsync(SessionAccessDto sessionAccessDto, CancellationToken cancellationToken = default);
}
