using Analytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Analytics.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _svc;
    public EventsController(IEventService svc) => _svc = svc;

    [HttpPost("record")]
    public async Task<IActionResult> Record([FromBody] EventRecordRequest req, CancellationToken ct)
    {
        await _svc.RecordAsync(req,
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            ct);
        return NoContent();
    }
}