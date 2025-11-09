namespace Shared.Contracts;

public class CampaignListResponse
{
    public IReadOnlyList<CampaignResponse> Items { get; init; } = Array.Empty<CampaignResponse>();
    public int Total { get; init; }
}