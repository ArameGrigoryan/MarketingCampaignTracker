using Campaign.Domain.Entities;
using Campaign.Domain.Enums;

namespace Campaign.Application.Interfaces;

public interface ICampaignRepository
{
    Task<CampaignEntity?> GetAsync(int id, CancellationToken ct = default);
    Task<(IReadOnlyList<CampaignEntity> items,int total)> ListAsync(int page,int pageSize, CampaignStatus? status, CancellationToken ct = default);
    Task AddAsync(CampaignEntity entity, CancellationToken ct = default);
    Task RemoveAsync(CampaignEntity entity, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<bool> LaunchAsync(int id, Func<Task<bool>> analyticsInit, CancellationToken ct = default);
}