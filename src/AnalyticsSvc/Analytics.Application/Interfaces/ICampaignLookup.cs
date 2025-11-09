namespace Analytics.Application.Interfaces;

public interface ICampaignLookup
{
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}