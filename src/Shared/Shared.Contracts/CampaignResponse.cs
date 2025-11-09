namespace Shared.Contracts;

public record CampaignResponse
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string TargetAudience { get; init; }
    public string Status { get; init; }
    public DateTimeOffset? LaunchedAt { get; init; }
    
    public CampaignResponse(int id, string name, string targetAudience, string status, DateTimeOffset? launchedAt)
    {
        Id = id;
        Name = name;
        TargetAudience = targetAudience;
        Status = status;
        LaunchedAt = launchedAt;
    }
}