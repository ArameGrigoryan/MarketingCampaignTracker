using Core.Api.Interfaces;
using Shared.Contracts;

namespace Core.Api.Gateways
{
    public sealed class AnalyticsGateway : IAnalyticsGateway
    {
        private readonly IAnalyticsClient _client;

        public AnalyticsGateway(IAnalyticsClient client)
        {
            _client = client;
        }

        public Task RecordEventAsync(EventRecordRequest request, CancellationToken ct = default)
            => _client.RecordEventAsync(request, ct);
    }
}