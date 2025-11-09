using Analytics.Api.Middleware;
using Analytics.Application.Interfaces;
using Analytics.Application.Services;
using Analytics.Application.Validation;
using Analytics.Infrastructure.Caching;
using Analytics.Infrastructure.Data;
using Analytics.Infrastructure.Gateways;
using Analytics.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// --- Config guardrails ---
var campaignBase = builder.Configuration["Services:CampaignBase"]
    ?? throw new InvalidOperationException("Missing configuration: Services:CampaignBase");
var redisConn = builder.Configuration["Redis:Connection"]
    ?? throw new InvalidOperationException("Missing configuration: Redis:Connection");
var dbConn = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string: Default");

// --- MVC + FluentValidation ---
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<EventRecordRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- HTTP client դեպի CampaignSvc (HEAD /exists) ---
builder.Services.AddHttpClient<ICampaignLookup, CampaignLookup>(c =>
{
    c.BaseAddress = new Uri(campaignBase);
    c.Timeout = TimeSpan.FromMilliseconds(300);
});

// --- Redis multiplexer ---
var muxerLazy = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConn));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => muxerLazy.Value);
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.ConnectionMultiplexerFactory = () => Task.FromResult(muxerLazy.Value);
});

// --- DbContext ---
builder.Services.AddDbContext<AnalyticsDb>(o =>
    o.UseNpgsql(dbConn /*.EnableRetryOnFailure()*/));

// --- Application layer ---
builder.Services.AddScoped<IEventService, EventService>();

// --- Infrastructure adapters ---
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICounterStore, RedisCounterStore>();

var app = builder.Build();

// auto-migrate analytics DB
using (var scope = app.Services.CreateScope())
{
    var adb = scope.ServiceProvider.GetRequiredService<AnalyticsDb>();
    await adb.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

// Bot filter միայն /events
app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/events"),
    branch => branch.UseMiddleware<BotFilterMiddleware>());

// Validation → 400, մնացածը → 500 JSON
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (FluentValidation.ValidationException ex)
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            code = "validation_failed",
            errors = ex.Errors.Select(e => e.ErrorMessage)
        });
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { code = "internal_error", message = ex.Message });
    }
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.Run();
