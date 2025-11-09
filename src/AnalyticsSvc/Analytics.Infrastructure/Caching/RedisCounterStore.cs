using StackExchange.Redis;
using Analytics.Application.Interfaces;

namespace Analytics.Infrastructure.Caching;

public class RedisCounterStore : ICounterStore
{
    private readonly IDatabase _db;
    public RedisCounterStore(IConnectionMultiplexer mux) => _db = mux.GetDatabase();

    private static string Key(int id, string type) => $"metrics:{id}:{type}";

    public Task InitAsync(int campaignId, CancellationToken ct = default)
    {
        var tasks = new[]
        {
            _db.StringSetAsync(Key(campaignId,"view"),       0, null, When.NotExists),
            _db.StringSetAsync(Key(campaignId,"click"),      0, null, When.NotExists),
            _db.StringSetAsync(Key(campaignId,"conversion"), 0, null, When.NotExists)
        };
        return Task.WhenAll(tasks);
    }

// IncrAsync՝ թող նախկինը. INCR TTL չի փոխում, բայց մենք TTL չենք դնում՝ խնդիր չկա
    public async Task<long> IncrAsync(int campaignId, string type, long by = 1, CancellationToken ct = default)
        => (long)await _db.StringIncrementAsync(Key(campaignId, type), by);

    public async Task<long?> GetAsync(int campaignId, string type, CancellationToken ct = default)
    {
        var v = await _db.StringGetAsync(Key(campaignId, type));
        return v.HasValue ? (long?)v : null;
    }

    public Task SetAsync(int campaignId, string type, long value, TimeSpan ttl, CancellationToken ct = default)
        => _db.StringSetAsync(Key(campaignId, type), value, ttl);
}