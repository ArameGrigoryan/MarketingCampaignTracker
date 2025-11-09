using Refit;
using Shared.Contracts;

namespace Core.Api.Interfaces;

public interface IAnalyticsClient
{
    [Post("/events/record")] 
    Task<ApiResponse<string?>> Record([Body] EventRecordRequest req, CancellationToken ct = default);
    [Get("/metrics/{campaignId}")] 
    Task<ApiResponse<MetricsResponse>> Get(int campaignId, CancellationToken ct = default);
    [Get("/metrics")]
    Task<ApiResponse<IReadOnlyList<MetricsResponse>>> GetBulk([Query(CollectionFormat.Multi)] int[] ids, CancellationToken ct = default);
}