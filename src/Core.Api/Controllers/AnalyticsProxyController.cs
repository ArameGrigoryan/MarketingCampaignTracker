using Core.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Core.Api.Controllers;

[ApiController]
[Route("gw/auth")]
public class AuthProxyController : ControllerBase
{
    private readonly ICampaignClient _campaign;
    public AuthProxyController(ICampaignClient campaign) => _campaign = campaign;

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(502)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var r = await _campaign.Login(req, ct);
        if (r.IsSuccessStatusCode && r.Content is not null) return Ok(r.Content);

        var detail = r.Error?.Message ?? r.ReasonPhrase ?? "upstream login failed";
        var status = (int)r.StatusCode == 0 ? 502 : (int)r.StatusCode;
        return Problem(statusCode: status, title: "CampaignSvc login error", detail: detail);
    }

}