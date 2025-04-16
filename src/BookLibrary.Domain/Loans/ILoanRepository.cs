using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<Loan>> GetActiveLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    Task<List<Loan>> GetOverdueLoansAsync(DateTime currentDate, CancellationToken cancellationToken = default);

    Task<Loan?> GetActiveLoanByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default);

    void Add(Loan loan);

    void Update(Loan loan);
}
