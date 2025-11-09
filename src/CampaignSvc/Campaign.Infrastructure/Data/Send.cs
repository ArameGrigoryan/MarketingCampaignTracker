using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Data;

public static class Seed
{
    public static async Task EnsureAsync(CampaignDb db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync())
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Admin#123"); // փոխիր .env-ով
            db.Users.Add(new User { Email = "admin@acme.io", PasswordHash = hash, Role = "Admin" });
            await db.SaveChangesAsync();
        }
    }
}