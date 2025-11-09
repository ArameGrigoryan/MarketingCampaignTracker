using Refit;
using Shared.Contracts;

namespace Core.Api.Interfaces;

public interface ICampaignClient
{
    [Post("/campaigns")]
    Task<ApiResponse<CampaignResponse>> Create([Body] CreateCampaignRequest req, CancellationToken ct = default);

    [Get("/campaigns")]
    Task<ApiResponse<PagedResponse<CampaignResponse>>> List(
        int page = 1,
        int pageSize = 20,
        string? status = null,
        string? audience = null,
        DateTimeOffset? launchedFrom = null,
        DateTimeOffset? launchedTo = null,
        CancellationToken ct = default);

    [Get("/campaigns/{id}")]
    Task<ApiResponse<CampaignResponse>> Get(int id, CancellationToken ct = default);

    [Put("/campaigns/{id}")]
    Task<ApiResponse<string?>> Update(int id, [Body] CreateCampaignRequest req, CancellationToken ct = default);

    [Delete("/campaigns/{id}")]
    Task<ApiResponse<string?>> Delete(int id, CancellationToken ct = default);

    [Post("/campaigns/{id}/launch")]
    Task<ApiResponse<string?>> Launch(int id, CancellationToken ct = default);

    [Post("/auth/login")]
    Task<ApiResponse<LoginResponse>> Login([Body] LoginRequest req, CancellationToken ct = default);
}
