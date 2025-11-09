using Refit;

namespace Campaign.Application.Interfaces;

public interface IAnalyticsGatewayHttp
{
    [Post("/admin/init/{campaignId}")]
    Task<ApiResponse<string?>> InitAsync(int campaignId, CancellationToken ct = default);
}