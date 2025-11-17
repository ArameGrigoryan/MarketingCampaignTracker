using Refit;
using Shared.Contracts;

namespace Core.Api.Interfaces;
public interface IAnalyticsClient
{
    [Post("/events/record")]
    Task RecordEventAsync([Body] EventRecordRequest request, CancellationToken ct = default);
    
    [Get("/dashboard/metrics")]
    Task<ApiResponse<IReadOnlyList<MetricsResponse>>> GetBulk(
        [Query] int[] ids,
        CancellationToken ct = default);
}
