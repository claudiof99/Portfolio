using UmaFestHub.Domain.Entities;
using System;

namespace UmaFestHub.Application.Interfaces
{
    public interface ICustomerService
    {
        Task PurchaseTicketAsync(User user, Guid ticketId, CancellationToken cancellationToken = default);
        Task AddToCartAsync(User user, Guid productId, int quantity, CancellationToken cancellationToken = default);
    }
}
