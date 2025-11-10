using Microsoft.AspNetCore.Mvc;
using Campaign.Application.IServiceInterfaces;
using LoginRequest    = Shared.Contracts.LoginRequest;
using RegisterRequest = Shared.Contracts.RegisterRequest;
using LoginResponse  = Shared.Contracts.LoginResponse;

namespace Campaign.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        try
        {
            var token = await _auth.LoginAsync(req, ct);
            return Ok(new LoginResponse(token));
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(204)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        try
        {
            await _auth.RegisterAsync(req, ct);
            return NoContent();
        }
        catch (System.InvalidOperationException)
        {
            return Conflict();
        }
    }
}
