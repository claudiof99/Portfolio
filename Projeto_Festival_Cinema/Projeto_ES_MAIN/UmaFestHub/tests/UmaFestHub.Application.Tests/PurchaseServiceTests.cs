/*
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Services;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace UmaFestHub.Application.Tests;

public class PurchaseServiceTests
{
	[Fact]
	public async Task CheckoutAsync_ComputesTotalAndStoresPurchase()
	{
		var repository = new InMemoryPurchaseRepository();
		var service = new PurchaseService(repository, new SuccessfulPaymentService(), new InMemoryCartRepository(), NullLogger<PurchaseService>.Instance);

		var purchaseId = await service.CheckoutAsync(Guid.NewGuid(),
		[
			new PurchaseItemDto(Guid.NewGuid(), 2, 10m),
			new PurchaseItemDto(Guid.NewGuid(), 1, 5m)
		]);

		var saved = repository.Stored.Single(x => x.Id == purchaseId);
		Assert.Equal(25m, saved.TotalAmount);
		Assert.Equal(PurchaseStatus.Completed, saved.Status);
	}

	private sealed class SuccessfulPaymentService : IPaymentSimulationService
	{
		public Task<PaymentResultDto> ProcessAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
			=> Task.FromResult(new PaymentResultDto(true, "sim-tx", null));
	}

	private sealed class InMemoryPurchaseRepository : IPurchaseRepository
	{
		public List<Purchase> Stored { get; } = new();

		public Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
			=> Task.FromResult<IReadOnlyList<Purchase>>(Stored.Where(x => x.UserId == userId).ToList());

		public Task<int> CountAsync(CancellationToken cancellationToken = default)
			=> Task.FromResult(Stored.Count);

		public Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
		{
			Stored.Add(purchase);
			return Task.CompletedTask;
		}
	}

	private sealed class InMemoryCartRepository : ICartRepository
	{
		public Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
			=> Task.FromResult<Cart?>(null);

		public Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
			=> Task.CompletedTask;

		public Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
			=> Task.CompletedTask;
	}
}
*/