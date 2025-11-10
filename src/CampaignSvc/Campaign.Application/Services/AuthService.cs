using System;
using System.Threading;
using System.Threading.Tasks;
using Campaign.Application.Interfaces;
using Campaign.Application.IServiceInterfaces;
using Shared.Abstractions.Security;
using LoginRequest    = Shared.Contracts.LoginRequest;
using RegisterRequest = Shared.Contracts.RegisterRequest;
using Campaign.Domain.Entities;

namespace Campaign.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtProvider _jwt;

    public AuthService(IUserRepository users, IJwtProvider jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<string> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var u = await _users.GetByEmailAsync(req.Email, ct);
        if (u is null) throw new UnauthorizedAccessException();

        if (!BCrypt.Net.BCrypt.Verify(req.Password, u.PasswordHash))
            throw new UnauthorizedAccessException();

        return _jwt.Generate(u.Id, u.Email, u.Role);
    }

    public async Task RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _users.GetByEmailAsync(req.Email, ct) is not null)
            throw new InvalidOperationException("User exists");

        var u = new User
        {
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = string.IsNullOrWhiteSpace(req.Role) ? "Marketing" : req.Role
        };

        await _users.AddAsync(u, ct);
    }
}