using Campaign.Domain.Entities;

namespace Campaign.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}