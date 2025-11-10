namespace Shared.Abstractions.Security;

public interface IJwtProvider
{
    string Generate(int userId, string email, string role);
}