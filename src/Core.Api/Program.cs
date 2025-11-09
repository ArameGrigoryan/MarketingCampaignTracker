using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;
using Core.Api.Interfaces;

var b = WebApplication.CreateBuilder(args);

b.Services.AddControllers();
b.Services.AddEndpointsApiExplorer();
b.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core Gateway", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, Name = "Authorization",
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type=ReferenceType.SecurityScheme, Id="Bearer"}}, Array.Empty<string>() }
    });
    c.CustomSchemaIds(t => t.FullName);
    c.IgnoreObsoleteActions();
});

// ----- Config sanity -----
string? campaignBase = b.Configuration["Services:CampaignBase"];
string? analyticsBase = b.Configuration["Services:AnalyticsBase"];
if (string.IsNullOrWhiteSpace(campaignBase) || string.IsNullOrWhiteSpace(analyticsBase))
{
    throw new InvalidOperationException("Missing Services:CampaignBase or Services:AnalyticsBase in configuration.");
}

// JWT key with safe fallback (>=32 bytes)
var key = b.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key))
{
    // dev-safe default
    key = "this_is_a_very_long_dev_secret_32b___fallback";
}
if (key.Length < 32) // HS256 needs >=128 bits
{
    key = key.PadRight(32, '_');
}

b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    });
b.Services.AddAuthorization();

// Forward caller Authorization to downstream
b.Services.AddHttpContextAccessor();
b.Services.AddTransient<AuthHeaderHandler>();

// Refit clients
b.Services.AddRefitClient<ICampaignClient>()
 .ConfigureHttpClient(c => c.BaseAddress = new Uri(campaignBase!))
 .AddHttpMessageHandler<AuthHeaderHandler>();

b.Services.AddRefitClient<IAnalyticsClient>()
 .ConfigureHttpClient(c => c.BaseAddress = new Uri(analyticsBase!))
 .AddHttpMessageHandler<AuthHeaderHandler>();

var app = b.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage(); 

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.RoutePrefix = "swagger";
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Core Gateway v1");
});

// Docker/dev-ում HTTPS redirect-ը հաճախ պետք չէ
// app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _ctx;
    public AuthHeaderHandler(IHttpContextAccessor ctx) => _ctx = ctx;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var h = _ctx.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(h)) req.Headers.TryAddWithoutValidation("Authorization", h);
        return base.SendAsync(req, ct);
    }
}
