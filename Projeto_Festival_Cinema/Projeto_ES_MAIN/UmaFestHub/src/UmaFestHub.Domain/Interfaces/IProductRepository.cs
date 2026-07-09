using UmaFestHub.Domain.Entities;
namespace UmaFestHub.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<DailyPass?> GetDailyPassAsync(Guid festivalId, CancellationToken cancellationToken = default);
        public Task<CompletePass?> GetCompletePassAsync(Guid festivalId, CancellationToken cancellationToken = default);
        public Task<Rental?> GetRentalAsync(Guid festivalId, CancellationToken cancellationToken = default);
        public Task<Ticket?> GetTicketAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
        Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}