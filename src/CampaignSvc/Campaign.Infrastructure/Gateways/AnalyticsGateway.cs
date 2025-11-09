using Campaign.Application.Interfaces;

namespace Campaign.Infrastructure.Gateways;

public sealed class AnalyticsGateway : IAnalyticsGateway
{
    private readonly IAnalyticsGatewayHttp _http;
    public AnalyticsGateway(IAnalyticsGatewayHttp http) => _http = http;

    public async Task<bool> InitMetricsAsync(int campaignId, CancellationToken ct = default)
    {
        var resp = await _http.InitAsync(campaignId, ct);
        return resp.IsSuccessStatusCode;
    }
}