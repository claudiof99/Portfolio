
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    
        => await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
        => await _dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<DailyPass?> GetDailyPassAsync(
        Guid festivalId, 
        CancellationToken cancellationToken = default)
        => await _dbContext.DailyPasses
            .Where(p => p.FestivalId == festivalId)
            .AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<CompletePass?> GetCompletePassAsync(
        Guid festivalId, 
        CancellationToken cancellationToken = default)
        => await _dbContext.CompletePasses
            .Where(p => p.FestivalId == festivalId)
            .AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<Rental?> GetRentalAsync(
        Guid festivalFilmId, 
        CancellationToken cancellationToken = default)
        => await _dbContext.Rentals
            .Where(p => p.FestivalFilmId == festivalFilmId)
            .AsNoTracking().FirstOrDefaultAsync(cancellationToken);


    public Task<Ticket?> GetTicketAsync(
        Guid sessionId, 
        CancellationToken cancellationToken = default)
        => _dbContext.Tickets
            .Where(t => t.SessionId == sessionId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Products
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public async Task AddAsync(
        Product product, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Product product, 
        CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products.FindAsync(id, cancellationToken);
        if (product is null) return;
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

