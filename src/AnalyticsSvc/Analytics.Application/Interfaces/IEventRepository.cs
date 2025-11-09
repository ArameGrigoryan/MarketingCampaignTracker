using Analytics.Domain.Entities;
using Analytics.Domain.Enums;

namespace Analytics.Application.Interfaces;

public interface IEventRepository
{
    Task AddAsync(InteractionEvent e, CancellationToken ct = default);
    Task<long> CountAsync(int campaignId, InteractionType type, CancellationToken ct = default);
}