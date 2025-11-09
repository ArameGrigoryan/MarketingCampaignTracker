using Analytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.Api.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly ICounterStore _counters;
    public AdminController(ICounterStore counters) => _counters = counters;

    [HttpPost("init/{campaignId:int}")]
    public async Task<IActionResult> Init(int campaignId, CancellationToken ct)
    {
        await _counters.InitAsync(campaignId, ct);
        return NoContent();
    }
}