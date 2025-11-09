using Campaign.Domain.Enums;

namespace Campaign.Domain.Entities;

public class CampaignEntity {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public DateTimeOffset? LaunchedAt { get; set; }
    public int CreatedBy { get; set; }
}