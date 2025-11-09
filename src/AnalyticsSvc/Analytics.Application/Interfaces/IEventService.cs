using Shared.Contracts;

namespace Analytics.Application.Interfaces;

public interface IEventService
{
    Task RecordAsync(EventRecordRequest req, string? userAgent, string? ip, CancellationToken ct = default);
    Task<MetricsResponse> GetMetricsAsync(int campaignId, CancellationToken ct = default);
}