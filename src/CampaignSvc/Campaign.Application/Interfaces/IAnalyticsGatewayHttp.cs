using Refit;
using Shared.Contracts;

namespace Campaign.Application.Interfaces;

public interface IAnalyticsGatewayHttp
{
    [Post("/admin/init/{campaignId}")]
    Task<ApiResponse<string?>> InitAsync(int campaignId, CancellationToken ct = default);
    
    [Post("/events/record")]
    Task RecordEventAsync([Body] EventRecordRequest request, CancellationToken ct = default);

}