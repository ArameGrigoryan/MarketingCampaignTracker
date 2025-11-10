using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Data;

public class CampaignDb : DbContext
{
    public CampaignDb(DbContextOptions<CampaignDb> opt) : base(opt) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CampaignEntity> Campaigns => Set<CampaignEntity>();
    public DbSet<ConversionType> ConversionTypes => Set<ConversionType>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired().HasMaxLength(256);
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).IsRequired().HasMaxLength(64);
        });

        b.Entity<CampaignEntity>()
            .Property(x => x.Status)
            .HasConversion<int>();

        b.Entity<ConversionType>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}