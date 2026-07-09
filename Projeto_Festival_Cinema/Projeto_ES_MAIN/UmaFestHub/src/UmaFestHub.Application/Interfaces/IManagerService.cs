using UmaFestHub.Domain.Entities;
using System;

namespace UmaFestHub.Application.Interfaces
{
    public interface IManagerService
    {
        Task CreateFestivalAsync(User user, string festivalName, CancellationToken cancellationToken = default);
        Task ManageSessionAsync(User user, Guid sessionId, CancellationToken cancellationToken = default);
    }
}
