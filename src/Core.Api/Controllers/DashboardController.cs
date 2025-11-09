using Core.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Core.Api.Controllers;

[ApiController]
[Route("dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IAnalyticsClient _analytics;
    public DashboardController(IAnalyticsClient analytics) => _analytics = analytics;

    [HttpGet("metrics")]
    [ProducesResponseType(typeof(IReadOnlyList<MetricsResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(502)]
    public async Task<IActionResult> Metrics([FromQuery] int[]? ids, CancellationToken ct)
    {
        if (ids is null || ids.Length == 0) return BadRequest("ids required");
        var r = await _analytics.GetBulk(ids, ct);
        if (r.IsSuccessStatusCode && r.Content is not null) return Ok(r.Content);

        var detail = r.Error?.Message ?? r.ReasonPhrase ?? "upstream metrics error";
        var status = (int)r.StatusCode == 0 ? 502 : (int)r.StatusCode;
        return Problem(statusCode: status, title: "AnalyticsSvc error", detail: detail);
    }

}