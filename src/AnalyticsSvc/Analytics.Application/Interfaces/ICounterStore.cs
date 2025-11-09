namespace Analytics.Application.Interfaces;

public interface ICounterStore
{
    Task InitAsync(int campaignId, CancellationToken ct = default);
    Task<long> IncrAsync(int campaignId, string type, long by = 1, CancellationToken ct = default);
    Task<long?> GetAsync(int campaignId, string type, CancellationToken ct = default);
    Task SetAsync(int campaignId, string type, long value, TimeSpan ttl, CancellationToken ct = default);
}