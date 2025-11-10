using Campaign.Application.Interfaces;
using Campaign.Domain.Entities;
using Campaign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CampaignDb _db;
    public UserRepository(CampaignDb db) => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
}