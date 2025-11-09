using Analytics.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Analytics.Infrastructure.Gateways;

public sealed class CampaignLookup : ICampaignLookup
{
    private readonly HttpClient _http;
    private readonly IDistributedCache _cache;

    public CampaignLookup(HttpClient http, IDistributedCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var cacheKey = $"campaign:exists:{id}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (cached is "1") return true;
        if (cached is "0") return false;

        using var req = new HttpRequestMessage(HttpMethod.Head, $"/campaigns/{id}/exists");
        using var resp = await _http.SendAsync(req, ct);

        // 5xx → մի քեշավորի և բարձրացրու սխալ, որպեսզի ingest-ը չդառնա “չկա”
        if ((int)resp.StatusCode >= 500)
            throw new HttpRequestException($"Upstream {resp.StatusCode}");

        var exists = resp.IsSuccessStatusCode; // 200=true, 404=false

        var ttl = exists ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(45);
        await _cache.SetStringAsync(cacheKey, exists ? "1" : "0",
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);

        return exists;
    }
}