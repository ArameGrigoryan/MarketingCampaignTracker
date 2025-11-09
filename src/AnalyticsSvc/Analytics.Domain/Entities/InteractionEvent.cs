using Analytics.Domain.Enums;

namespace Analytics.Domain.Entities;

public class InteractionEvent {
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public InteractionType Type { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
    public string? Referrer { get; set; }
    public int? ConversionTypeId { get; set; }
}