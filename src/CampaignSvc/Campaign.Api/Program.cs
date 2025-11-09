using System.Text;
using Campaign.Application.Interfaces;
using Campaign.Application.IServiceInterfaces;
using Campaign.Application.Services;
using Campaign.Application.Validation;
using Campaign.Infrastructure.Data;
using Campaign.Infrastructure.Gateways;
using Campaign.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Controllers + FluentValidation (նոր API)
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCampaignRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Campaign API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, Name = "Authorization",
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type=ReferenceType.SecurityScheme, Id="Bearer"}}, Array.Empty<string>() }
    });
});

// DbContext
builder.Services.AddDbContext<CampaignDb>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositories + Gateways
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IAnalyticsGateway, AnalyticsGateway>();

// Refit HTTP client to AnalyticsSvc
builder.Services.AddRefitClient<IAnalyticsGatewayHttp>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["Services:AnalyticsBase"]!));

// Application services
builder.Services.AddScoped<ICampaignService, CampaignService>();

// JWT auth
var keyBytes = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "supersecret_supersecret_123");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Docker-ում HTTPS չունես՝ UseHttpsRedirection-ը կամ հանիր, կամ պահիր միայն non-docker dev-ի համար
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

// DB migrate + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampaignDb>();
    await Seed.EnsureAsync(db);
}

app.Run();
