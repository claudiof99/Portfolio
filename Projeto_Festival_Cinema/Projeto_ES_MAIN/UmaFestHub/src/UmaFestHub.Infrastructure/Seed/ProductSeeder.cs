using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Application.Factories;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Seed;
public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // await SeedPassesAsync(context);
        await SeedRentalsAsync(context);
        await SeedTicketsAsync(context);
    }

    private static async Task SeedRentalsAsync(AppDbContext context)
    {
        var existingRentalFilmIds = await context.Products
            .OfType<Rental>()
            .Select(r => r.FestivalFilmId)
            .ToListAsync();
            
        var festivalFilms = await context.FestivalFilms.ToListAsync();

        foreach (var ff in festivalFilms)
        {
            if (!existingRentalFilmIds.Contains(ff.Id))
            {
                context.Products.Add(
                    new Rental(
                    festivalFilmId: ff.Id,
                    price: 4.99m,
                    duration: new Duration(48, DurationUnit.Hours))
                );
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedTicketsAsync(AppDbContext context)
    {
        if (await context.Products.OfType<Ticket>().AnyAsync())
            return;

        var sessions = await context.Sessions
            .Where(s => s.SessionType != SessionType.AccessWindow)
            .ToListAsync();

        foreach (var session in sessions)
        {
            context.Products.Add(
                new Ticket(
                    sessionId: session.Id,
                    price: 7.50m)
            );
        }

        await context.SaveChangesAsync();
    }

}
