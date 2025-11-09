using Campaign.Application.Interfaces;
using Campaign.Application.IServiceInterfaces;
using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Shared.Contracts;

namespace Campaign.Application.Services;

public sealed class CampaignService : ICampaignService
{
    private readonly ICampaignRepository _repo;
    private readonly IAnalyticsGateway _analytics;

    public CampaignService(ICampaignRepository repo, IAnalyticsGateway analytics)
    {
        _repo = repo;
        _analytics = analytics;
    }

    public async Task<CampaignResponse> CreateAsync(CreateCampaignRequest req, CancellationToken ct = default)
    {
        var e = new CampaignEntity
            { Name = req.Name, TargetAudience = req.TargetAudience, Status = CampaignStatus.Draft };
        await _repo.AddAsync(e, ct);
        await _repo.SaveChangesAsync(ct);
        return Map(e);
    }

    public async Task<(IReadOnlyList<CampaignResponse> items, int total)> ListAsync(int page, int pageSize,
        string? status, CancellationToken ct = default)
    {
        CampaignStatus? st = null;
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<CampaignStatus>(status, true, out var parsed)) st = parsed;
        var (items, total) = await _repo.ListAsync(page, pageSize, st, ct);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<CampaignResponse?> GetAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetAsync(id, ct);
        return e is null ? null : Map(e);
    }

    public async Task UpdateAsync(int id, CreateCampaignRequest req, CancellationToken ct = default)
    {
        var e = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException();
        e.Name = req.Name;
        e.TargetAudience = req.TargetAudience;
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException();
        await _repo.RemoveAsync(e, ct);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task LaunchAsync(int id, CancellationToken ct = default)
    {
        var ok = await _repo.LaunchAsync(id, () => _analytics.InitMetricsAsync(id, ct), ct);
        if (!ok)
            throw new InvalidOperationException("metrics-init-failed");
    }
    private static CampaignResponse Map(CampaignEntity e) =>
        new(e.Id, e.Name, e.TargetAudience, e.Status.ToString(), e.LaunchedAt);

}