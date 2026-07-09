using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Application.Factories;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly Dictionary<string, ProductStore> _stores;


        public ProductService(
            IProductRepository productRepository, 
            IEnumerable<ProductStore> stores)
        {
            _productRepository = productRepository;
            _stores = stores.ToDictionary(s => s.ProductType.ToLowerInvariant());
        }

        public async Task<ProductDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return null;

            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return null;

            return new ProductDto(product.Id, product.ProductType, product.Price);
        }
        
        public async Task<DailyPassDto?> GetDailyPassDtoAsync(Guid festivalId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetDailyPassAsync(festivalId, cancellationToken);
            if(product is null || product.FestivalId != festivalId)
            {
                return null;
            }
            else
            {
                var dto = new DailyPassDto(
                    product.Id,
                    product.Price,
                    product.ProductType,
                    product.FestivalId,
                    product.DateUtc
                );
                return dto;
            }
            

        }

        public async Task<CompletePassDto?> GetCompletePassDtoAsync(Guid festivalId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetCompletePassAsync(festivalId, cancellationToken);
            if(product is null || product.FestivalId != festivalId)
            {
                return null;
            }
            else
            {
                var dto = new CompletePassDto(
                    product.Id,
                    product.Price,
                    product.ProductType,
                    product.FestivalId
                );
                return dto;
            }
        }

        public async Task<RentalDto?> GetRentalDtoAsync(Guid festivalFilmId, CancellationToken cancellationToken = default)
        {
            var rental = await _productRepository.GetRentalAsync(festivalFilmId, cancellationToken);
            if (rental is null)
                return null;

            return new RentalDto(
                rental.Id,
                rental.ProductType,
                rental.Price,
                rental.FestivalFilmId,
                rental.Duration.Value,
                rental.Duration.Unit.ToString());
        }

        public async Task<TicketDto?> GetTicketDtoAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var ticket = await _productRepository.GetTicketAsync(sessionId, cancellationToken);
            if (ticket is null)
                return null;

            return new TicketDto(
                ticket.Id,
                ticket.SessionId,
                ticket.ProductType,
                ticket.Price,
                ticket.TicketNumber
            );
        }



        public async Task CreateRentalAsync(RentalDto rentalDto, CancellationToken cancellationToken = default)
        {

            var duration = new Duration
            {
                Value = rentalDto.DurationValue,
                Unit = Enum.Parse<DurationUnit>(rentalDto.DurationUnit, ignoreCase: true)
            };

            var rental = new Rental(rentalDto.FestivalFilmId, rentalDto.Price, duration);
            await _productRepository.AddAsync(rental, cancellationToken);
        }

        public async Task CreateDailyPassAsync(DailyPassDto dailyPassDto, CancellationToken cancellationToken = default)
        {
            var pass = new DailyPass(
                dailyPassDto.FestivalId, 
                dailyPassDto.Price, 
                dailyPassDto.DateUtc
            );
            if (pass.Id == Guid.Empty)
            {
                pass.Id = Guid.NewGuid();
            }
            await _productRepository.AddAsync(pass, cancellationToken);
        }

        public async Task CreateCompletePassAsync(CompletePassDto completePassDto, CancellationToken cancellationToken = default)
        {
            var pass = new CompletePass(
                completePassDto.FestivalId, 
                completePassDto.Price
            );
            if (pass.Id == Guid.Empty)
            {
                pass.Id = Guid.NewGuid();
            }
            await _productRepository.AddAsync(pass, cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetByIdsAsync(ids, cancellationToken);
            return products
                .Select(p => new ProductDto(p.Id, p.ProductType, p.Price))
                .ToList();
        }

    }
}
