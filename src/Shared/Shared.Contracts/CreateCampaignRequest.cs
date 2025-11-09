namespace Shared.Contracts;

public record CreateCampaignRequest
{
    public string Name { get; init; }
    public string TargetAudience { get; init; }

    public CreateCampaignRequest(string name, string targetAudience)
    {
        Name = name;
        TargetAudience = targetAudience;
    }
}