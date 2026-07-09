
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Interfaces
{
    public interface IProductService
    {
        public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<ProductDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        public Task<DailyPassDto?> GetDailyPassDtoAsync(Guid festivalId, CancellationToken cancellationToken = default);
        public Task<CompletePassDto?> GetCompletePassDtoAsync(Guid festivalId, CancellationToken cancellationToken = default);
        public Task<RentalDto?> GetRentalDtoAsync(Guid festivalFilmId, CancellationToken cancellationToken = default);
        public Task<TicketDto?> GetTicketDtoAsync(Guid sessionId, CancellationToken cancellationToken = default);
        public Task CreateRentalAsync(RentalDto rentalDto, CancellationToken cancellationToken = default);
        public Task CreateDailyPassAsync(DailyPassDto dailyPassDto, CancellationToken cancellationToken = default);
        public Task CreateCompletePassAsync(CompletePassDto completePassDto, CancellationToken cancellationToken = default);
    }
}
