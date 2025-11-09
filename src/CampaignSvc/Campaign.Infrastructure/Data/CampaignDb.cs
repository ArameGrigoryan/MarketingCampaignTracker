using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Data;

public class CampaignDb : DbContext {
    
    public CampaignDb(DbContextOptions<CampaignDb> opt) : base(opt) { }
    
    public DbSet<User> Users => Set<User>();
    
    public DbSet<CampaignEntity> Campaigns => Set<CampaignEntity>();
    
    public DbSet<ConversionType> ConversionTypes => Set<ConversionType>();
    
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        
        b.Entity<CampaignEntity>().Property(x => x.Status).HasConversion<int>();
        
        b.Entity<ConversionType>().HasIndex(x => x.Name).IsUnique();
    }
}