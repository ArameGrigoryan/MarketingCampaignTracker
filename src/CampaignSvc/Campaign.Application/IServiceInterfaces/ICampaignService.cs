using Shared.Contracts;

namespace Campaign.Application.IServiceInterfaces;

public interface ICampaignService
{
    Task<CampaignResponse> CreateAsync(CreateCampaignRequest req, CancellationToken ct=default);
    Task<(IReadOnlyList<CampaignResponse> items,int total)> ListAsync(int page,int pageSize,string? status, CancellationToken ct=default);
    Task<CampaignResponse?> GetAsync(int id, CancellationToken ct=default);
    Task UpdateAsync(int id, CreateCampaignRequest req, CancellationToken ct=default);
    Task DeleteAsync(int id, CancellationToken ct=default);

    Task LaunchAsync(int id, CancellationToken ct = default);

}