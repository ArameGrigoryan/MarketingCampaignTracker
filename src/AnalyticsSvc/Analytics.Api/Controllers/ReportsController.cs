using Analytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Analytics.Api.Controllers;

[ApiController]
[Route("metrics")]
public class ReportsController : ControllerBase
{
    private readonly IEventService _svc;
    public ReportsController(IEventService svc) => _svc = svc;

    [HttpGet("{campaignId:int}")]
    [ProducesResponseType(typeof(MetricsResponse), 200)]
    public Task<MetricsResponse> Get(int campaignId, CancellationToken ct)
        => _svc.GetMetricsAsync(campaignId, ct);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MetricsResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetBulk([FromQuery] int[] ids, CancellationToken ct)
    {
        if (ids is null || ids.Length == 0) return BadRequest("ids required"); 
        var tasks = ids.Select(id => _svc.GetMetricsAsync(id, ct)).ToArray();
        var results = await Task.WhenAll(tasks);
        return Ok(results);
    }
}