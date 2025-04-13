using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans.Events;

namespace BookLibrary.Domain.Loans;

public class Loan : Entity
{
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public LoanPeriod Period { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public bool IsReturned => ReturnedAt.HasValue;

    // Parameterless constructor for EF Core
    private Loan()
    {
    }

    private Loan(Guid id, Guid userId, Guid bookId, LoanPeriod period) : base(id)
    {
        UserId = userId;
        BookId = bookId;
        Period = period;
    }

    public static Result<Loan> Create(Guid id, Guid userId, Guid bookId, LoanPeriod period)
    {

        var loan = new Loan(id, userId, bookId, period);

        loan.RaiseDomainEvent(new LoanCreatedDomainEvent(loan));

        return Result.Success(loan);
    }

    public void MarkAsReturned(DateTime returnedAt)
    {
        if (IsReturned)
        {
            throw new InvalidOperationException("Loan already returned.");
        }

        ReturnedAt = returnedAt;
        RaiseDomainEvent(new LoanReturnedDomainEvent(this));
    }

    public Result Extend(LoanPeriod extension)
    {
        if (extension.StartDate < Period.EndDate)
        {
            return Result.Failure(LoanErrors.InvalidExtension);
        }

        if (IsReturned)
        {
            return Result.Failure(LoanErrors.LoanAlreadyReturned);
        }

        Period = extension;
        RaiseDomainEvent(new LoanExtendedDomainEvent(this, extension));
        return Result.Success();
    }
}
