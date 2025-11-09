using Core.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Refit;

namespace Core.Api.Controllers;

[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Marketing,Admin")]
[Route("gw/campaigns")]
public class CampaignsProxyController : ControllerBase
{
    private readonly ICampaignClient _cli;
    public CampaignsProxyController(ICampaignClient cli) => _cli = cli;

    private IActionResult Map<T>(ApiResponse<T> r)
    {
        if (r.IsSuccessStatusCode)
        {
            if ((int)r.StatusCode == 204 || r.Content is null)
                return StatusCode((int)r.StatusCode);
            return StatusCode((int)r.StatusCode, r.Content);
        }

        var msg = r.Error?.Message ?? r.ReasonPhrase ?? "Upstream error";
        return Problem(statusCode: (int)r.StatusCode, title: "CampaignSvc error", detail: msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCampaignRequest req, CancellationToken ct)
    {
        var r = await _cli.Create(req, ct);
        return Map(r);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CampaignResponse>), 200)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null, [FromQuery] string? audience = null,
        [FromQuery] DateTimeOffset? launchedFrom = null, [FromQuery] DateTimeOffset? launchedTo = null,
        CancellationToken ct = default)
    {
        var r = await _cli.List(page, pageSize, status, audience, launchedFrom, launchedTo, ct);
        return Map(r);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var r = await _cli.Get(id, ct);
        return Map(r);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCampaignRequest req, CancellationToken ct)
    {
        var r = await _cli.Update(id, req, ct);
        return Map(r);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var r = await _cli.Delete(id, ct);
        return Map(r);
    }

    [HttpPost("{id:int}/launch")]
    public async Task<IActionResult> Launch(int id, CancellationToken ct)
    {
        var r = await _cli.Launch(id, ct);
        return Map(r);
    }
}
