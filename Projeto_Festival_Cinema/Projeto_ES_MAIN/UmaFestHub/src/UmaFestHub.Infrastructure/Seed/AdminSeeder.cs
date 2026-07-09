using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Application.Factories;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Seed;

public static class AdminSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Users.AnyAsync(u => u.Email == "admin@umafesthub.com"))
        {
            var (adminHash, adminSalt) = HashPassword("Admin@123");
            context.Users.Add(UserFactory.CreateAdmin(
                name: "Super Admin",
                email: "admin@umafesthub.com",
                passwordHash: adminHash,
                passwordSalt: adminSalt
            ));
        }

        if (!await context.Users.AnyAsync(u => u.Email == "organizer@umafesthub.com"))
        {
            var (organizerHash, organizerSalt) = HashPassword("Organizer@123");
            context.Users.Add(UserFactory.CreateOrganizer(
                name: "Festival Organizer",
                email: "organizer@umafesthub.com",
                passwordHash: organizerHash,
                passwordSalt: organizerSalt
            ));
        }

        if (!await context.Users.AnyAsync(u => u.Email == "customer@umafesthub.com"))
        {
            var (customerHash, customerSalt) = HashPassword("Customer@123");
            context.Users.Add(UserFactory.CreateCustomer(
                name: "Test Customer",
                email: "customer@umafesthub.com",
                passwordHash: customerHash,
                passwordSalt: customerSalt
            ));
        }

        await context.SaveChangesAsync();
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);

        var hash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            100_000,
            HashAlgorithmName.SHA256,
            32));

        return (hash, salt);
    }
}