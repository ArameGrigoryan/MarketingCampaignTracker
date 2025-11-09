using Analytics.Application.Interfaces;
using Analytics.Domain.Entities;
using Analytics.Domain.Enums;
using Analytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AnalyticsDb _db;
    public EventRepository(AnalyticsDb db) => _db = db;

    public async Task AddAsync(InteractionEvent e, CancellationToken ct = default)
    {
        _db.InteractionEvents.Add(e);
        await _db.SaveChangesAsync(ct);
    }

    public Task<long> CountAsync(int campaignId, InteractionType type, CancellationToken ct = default) =>
        _db.InteractionEvents.LongCountAsync(x => x.CampaignId == campaignId && x.Type == type, ct);
}