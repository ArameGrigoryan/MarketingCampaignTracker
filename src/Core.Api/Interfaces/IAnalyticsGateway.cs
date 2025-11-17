using Shared.Contracts;

namespace Core.Api.Interfaces
{
    public interface IAnalyticsGateway
    {
        Task RecordEventAsync(EventRecordRequest request, CancellationToken ct = default);
    }
}