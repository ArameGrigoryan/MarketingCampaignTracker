using System.Net.Http.Json;
using Campaign.Application.Interfaces;
using Shared.Contracts;

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
    
    public Task RecordEventAsync(EventRecordRequest request, CancellationToken ct = default)
        => _http.RecordEventAsync(request, ct);

}