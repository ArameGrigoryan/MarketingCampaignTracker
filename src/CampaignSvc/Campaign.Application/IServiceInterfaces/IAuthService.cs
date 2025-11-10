using LoginRequest    = Shared.Contracts.LoginRequest;
using RegisterRequest = Shared.Contracts.RegisterRequest;

namespace Campaign.Application.IServiceInterfaces;
public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest req, CancellationToken ct);
    Task RegisterAsync(RegisterRequest req, CancellationToken ct);
}