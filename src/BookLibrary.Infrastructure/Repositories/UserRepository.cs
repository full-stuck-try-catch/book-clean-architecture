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
            .FirstOrDefaultAsync(user => user.Email.Value == email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return !await DbContext.Set<User>()
            .AnyAsync(user => user.Email.Value == email, cancellationToken);
    }
}
