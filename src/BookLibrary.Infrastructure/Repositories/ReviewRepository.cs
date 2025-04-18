using BookLibrary.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

internal sealed class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Review?> GetByBookAndUserAsync(Guid bookId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Review>()
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);
    }

    public async Task<List<Review>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Review>()
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Review>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Review>()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Review review)
    {
        DbContext.Set<Review>().Remove(review);
    }
}
