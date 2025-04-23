using BookLibrary.Domain.Loans;
using BookLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

internal sealed class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Loans
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Loan>> GetActiveLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Loans
            .Where(l => l.UserId == userId && l.ReturnedAt == null)
            .OrderByDescending(l => l.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Loans
            .FirstOrDefaultAsync(l => l.UserId == userId && l.BookId == bookId && l.ReturnedAt == null, cancellationToken);
    }
}
