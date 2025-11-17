using Shared.Contracts;

namespace Campaign.Application.Interfaces;

public interface IAnalyticsGateway
{
    Task<bool> InitMetricsAsync(int campaignId, CancellationToken ct = default);
    
    Task RecordEventAsync(EventRecordRequest request, CancellationToken ct = default);

}