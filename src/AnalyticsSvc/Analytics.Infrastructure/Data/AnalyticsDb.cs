using Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Data;

public class AnalyticsDb : DbContext
{
    public AnalyticsDb(DbContextOptions<AnalyticsDb> opt) : base(opt) { }

    public DbSet<InteractionEvent> InteractionEvents => Set<InteractionEvent>();
    public DbSet<CampaignProjection> CampaignProjections => Set<CampaignProjection>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<InteractionEvent>(e =>
        {
            e.Property(x => x.Type).HasConversion<int>();

            e.HasIndex(x => new { x.CampaignId, x.OccurredAt });
            e.HasIndex(x => new { x.CampaignId, x.Type });
        });

        b.Entity<CampaignProjection>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Status).HasMaxLength(50);
        });
    }
}
