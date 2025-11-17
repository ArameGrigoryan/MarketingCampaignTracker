using System.Text;
using Campaign.Application.Interfaces;
using Campaign.Application.IServiceInterfaces;
using Campaign.Application.Services;
using Campaign.Application.Validation;
using Campaign.Infrastructure.Auth;
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
using Shared.Abstractions.Security;

var builder = WebApplication.CreateBuilder(args);

// MVC + Validation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCampaignRequestValidator>();

// Swagger + Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Campaign API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<CampaignDb>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Services & Gateways
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IAnalyticsGateway, AnalyticsGateway>();
builder.Services.AddRefitClient<IAnalyticsGatewayHttp>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["Services:AnalyticsBase"]!));
builder.Services.AddScoped<ICampaignService, CampaignService>();

// Auth
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "supersecret_supersecret_123");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

// Seed (ընտրովի)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampaignDb>();
    await Seed.EnsureAsync(db);
}

app.Run();
