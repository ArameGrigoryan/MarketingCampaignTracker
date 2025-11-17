using Microsoft.AspNetCore.Mvc;
using Refit;
using Core.Api.Interfaces;   // IAnalyticsClient
using Shared.Contracts;      

namespace Core.Api.Controllers;

[ApiController]
[Route("gw/events")]
public sealed class EventsProxyController : ControllerBase
{
    private readonly IAnalyticsClient _analytics;

    public EventsProxyController(IAnalyticsClient analytics)
    {
        _analytics = analytics;
    }

    [HttpPost("record")]
    public async Task<IActionResult> Record([FromBody] EventRecordRequest req, CancellationToken ct)
    {
        try
        {
            await _analytics.RecordEventAsync(req, ct);
            return Accepted();
        }
        catch (ApiException ex)
        {
            return StatusCode(
                (int)ex.StatusCode,
                new
                {
                    title = "AnalyticsSvc events error",
                    status = (int)ex.StatusCode,
                    content = ex.Content,
                    message = ex.Message
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    title = "Core Gateway internal error",
                    message = ex.Message,
                    type = ex.GetType().FullName,
                    stack = ex.StackTrace
                });
        }
    }
}