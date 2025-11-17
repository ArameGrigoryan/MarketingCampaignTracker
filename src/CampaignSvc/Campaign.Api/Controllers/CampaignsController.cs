using Campaign.Application.IServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Campaign.Api.Controllers;

[ApiController]
[Route("campaigns")]
[Authorize(Roles = "Marketing,Admin")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _svc;
    public CampaignsController(ICampaignService svc) => _svc = svc;

    [HttpHead("{id:int}/exists")]
    [AllowAnonymous]
    public async Task<IActionResult> Exists(int id, CancellationToken ct)
        => (await _svc.GetAsync(id, ct)) is null ? NotFound() : Ok();


    [HttpPost]
    [ProducesResponseType(typeof(CampaignResponse), 200)]
    public Task<CampaignResponse> Create([FromBody] CreateCampaignRequest req, CancellationToken ct)
        => _svc.CreateAsync(req, ct);

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CampaignResponse>), 200)]
    public async Task<PagedResponse<CampaignResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _svc.ListAsync(page, pageSize, status, ct);
        return new PagedResponse<CampaignResponse> { Items = items, Total = total };
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CampaignResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var res = await _svc.GetAsync(id, ct);
        return res is null ? NotFound() : Ok(res);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCampaignRequest req, CancellationToken ct)
    {
        await _svc.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/launch")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Launch(int id, CancellationToken ct)
    {
        await _svc.LaunchAsync(id, ct);
        return NoContent();
    }
}