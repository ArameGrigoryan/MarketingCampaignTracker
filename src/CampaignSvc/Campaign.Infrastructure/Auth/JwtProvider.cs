using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Abstractions.Security;

namespace Campaign.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _config;
    public JwtProvider(IConfiguration config) => _config = config;

    public string Generate(int userId, string email, string role)
    {
        var key = _config["Jwt:Key"] 
                  ?? throw new InvalidOperationException("Missing Jwt:Key in configuration");

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}