using Analytics.Application.Interfaces;
using Analytics.Domain.Entities;
using Analytics.Domain.Enums;
using FluentValidation;
using Shared.Contracts;

namespace Analytics.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _repo;
    private readonly ICounterStore _counters;
    private readonly IValidator<EventRecordRequest> _validator;
    private readonly ICampaignLookup _lookup;

    public EventService(IEventRepository repo, ICounterStore counters, IValidator<EventRecordRequest> validator, ICampaignLookup lookup)
    {
        _repo = repo;
        _counters = counters;
        _validator = validator;
        _lookup = lookup;
    }

    public async Task RecordAsync(EventRecordRequest req, string? userAgent, string? ip, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(req, ct);

        var exists = await _lookup.ExistsAsync(req.CampaignId, ct);
        if (!exists) throw new ValidationException($"Unknown CampaignId {req.CampaignId}");
        
        var entity = new InteractionEvent
        {
            CampaignId = req.CampaignId,
            Type = Enum.Parse<InteractionType>(req.Type, true),
            ConversionTypeId = req.ConversionTypeId,
            OccurredAt = req.OccurredAt ?? DateTimeOffset.UtcNow,
            Referrer = req.Referrer,
            UserAgent = userAgent,
            Ip = ip
        };

        await _repo.AddAsync(entity, ct);

        await _counters.IncrAsync(req.CampaignId, req.Type.ToLowerInvariant(), 1, ct);
    }

    public async Task<MetricsResponse> GetMetricsAsync(int campaignId, CancellationToken ct = default)
    {
        async Task<long> Read(string type, InteractionType t)
        {
            var cached = await _counters.GetAsync(campaignId, type, ct);
            if (cached.HasValue) return cached.Value;

            var val = await _repo.CountAsync(campaignId, t, ct);
            await _counters.SetAsync(campaignId, type, val, TimeSpan.FromHours(12), ct);
            return val;
        }

        var views = await Read("view", InteractionType.View);
        var clicks = await Read("click", InteractionType.Click);
        var convs  = await Read("conversion", InteractionType.Conversion);

        return new MetricsResponse(campaignId, views, clicks, convs);
    }
}
