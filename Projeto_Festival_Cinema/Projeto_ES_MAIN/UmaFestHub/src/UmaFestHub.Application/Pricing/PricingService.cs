using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Pricing;

public sealed class PricingService : IPricingService
{
    private readonly IPricingStrategy _strategy;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IUserRepository _userRepository;

    public PricingService(
        IPricingStrategy strategy,
        IFestivalRepository festivalRepository,
        IUserRepository userRepository)
    {
        _strategy = strategy;
        _festivalRepository = festivalRepository;
        _userRepository = userRepository;
    }

    public async Task<decimal> GetPriceAsync(Product product, Guid userId, DateTime purchaseDate, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        Festival? festival = null;
        var festivalId = product.GetFestivalId();
        if (festivalId.HasValue)
            festival = await _festivalRepository.GetByIdAsync(festivalId.Value, cancellationToken);

        var context = new PricingContext
        {
            User = user,
            PurchaseDate = purchaseDate,
            Festival = festival
        };

        return _strategy.Calculate(product, context);
    }
}
