namespace Shared.Contracts;

public record MetricsResponse
{
    public int CampaignId { get; init; }
    public long Views { get; init; }
    public long Clicks { get; init; }
    public long Conversions { get; init; }

    public MetricsResponse(int campaignId, long views, long clicks, long conversions)
    {
        CampaignId = campaignId;
        Views = views;
        Clicks = clicks;
        Conversions = conversions;
    }
}