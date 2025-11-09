using Campaign.Application.Interfaces;
using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Campaign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly CampaignDb _db;
    public CampaignRepository(CampaignDb db) => _db = db;

    public Task<CampaignEntity?> GetAsync(int id, CancellationToken ct=default) =>
        _db.Campaigns.FirstOrDefaultAsync(x=>x.Id==id, ct);

    public async Task<(IReadOnlyList<CampaignEntity> items,int total)> ListAsync(int page,int pageSize, CampaignStatus? status, CancellationToken ct=default)
    {
        var q = _db.Campaigns.AsQueryable();
        if (status.HasValue) q = q.Where(x=>x.Status==status.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct);
        return (items,total);
    }

    public async Task AddAsync(CampaignEntity e, CancellationToken ct=default)
    {
        await _db.Campaigns.AddAsync(e, ct);
    }

    public Task RemoveAsync(CampaignEntity e, CancellationToken ct=default)
    {
        _db.Campaigns.Remove(e);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct=default) => _db.SaveChangesAsync(ct);
    
    public async Task<bool> LaunchAsync(int id, Func<Task<bool>> analyticsInit, CancellationToken ct = default)
    {
        var e = await _db.Campaigns.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException();

        if (e.Status != CampaignStatus.Draft)
            throw new InvalidOperationException("invalid");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        e.Status = CampaignStatus.Active;
        e.LaunchedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        var ok = await analyticsInit();
        if (!ok)
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        await tx.CommitAsync(ct);
        return true;
    }
}