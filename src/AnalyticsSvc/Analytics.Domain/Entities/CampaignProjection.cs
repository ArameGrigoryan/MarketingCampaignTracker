namespace Analytics.Domain.Entities;

public class CampaignProjection
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? LaunchedAt { get; set; }
}