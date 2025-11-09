namespace Analytics.Api.Middleware;

public class BotFilterMiddleware {
    private readonly RequestDelegate _next;
    private readonly string[] _deny;
    public BotFilterMiddleware(RequestDelegate next, IConfiguration cfg) {
        _next = next;
        _deny = cfg.GetSection("BotFilter:Denied").Get<string[]>() ?? Array.Empty<string>();
    }
    public async Task Invoke(HttpContext ctx) {
        var ua = ctx.Request.Headers.UserAgent.ToString().ToLowerInvariant();
        if (_deny.Any(p => ua.Contains(p))) { ctx.Response.StatusCode = 204; return; }
        await _next(ctx);
    }
}