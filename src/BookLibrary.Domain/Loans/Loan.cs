using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans.Events;

namespace BookLibrary.Domain.Loans;

public sealed class Loan : Entity
{
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public LoanPeriod Period { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public bool IsReturned => ReturnedAt.HasValue;

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

    public Result MarkAsReturned(DateTime returnedAt)
    {
        if (IsReturned)
        {
            return Result.Failure(LoanErrors.LoanAlreadyReturned);
        }

        ReturnedAt = returnedAt;
        RaiseDomainEvent(new LoanReturnedDomainEvent(this));

        return Result.Success();
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
