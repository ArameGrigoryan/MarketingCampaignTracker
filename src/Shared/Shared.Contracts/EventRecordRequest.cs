namespace Shared.Contracts;

public record EventRecordRequest
{
    public int CampaignId { get; init; }
    public string Type { get; init; }
    public int? ConversionTypeId { get; init; }
    public DateTimeOffset? OccurredAt { get; init; }
    public string? Referrer { get; init; }

    public EventRecordRequest(int campaignId, string type, int? conversionTypeId, DateTimeOffset? occurredAt, string? referrer)
    {
        CampaignId = campaignId;
        Type = type;
        ConversionTypeId = conversionTypeId;
        OccurredAt = occurredAt;
        Referrer = referrer;
    }
}