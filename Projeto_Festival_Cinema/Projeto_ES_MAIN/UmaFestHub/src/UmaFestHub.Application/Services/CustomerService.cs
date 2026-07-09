using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Application.Security;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ICartService _cartService;

        public CustomerService(IPurchaseService purchaseService, ICartService cartService)
        {
            _purchaseService = purchaseService;
            _cartService = cartService;
        }

        public async Task PurchaseTicketAsync(User user, Guid ticketId, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(user, UserRole.Customer);
            // Route to actual purchase logic
        }

        public async Task AddToCartAsync(User user, Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            PermissionGuard.EnsureRole(user, UserRole.Customer);
            await _cartService.AddProductAsync(user.Id, productId, quantity, cancellationToken);
        }
    }
}
