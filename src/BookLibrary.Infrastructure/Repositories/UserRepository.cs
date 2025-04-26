using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await DbContext.Set<User>()
            .FirstOrDefaultAsync(user => user.Email == new Email(email), cancellationToken);
    }

    public async Task<Role?> GetUserRole(string roleName, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Role>()
            .FirstOrDefaultAsync(role => role.Name == roleName, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return !await DbContext.Set<User>()
            .AnyAsync(user => user.Email == new Email(email), cancellationToken);
    }
}
