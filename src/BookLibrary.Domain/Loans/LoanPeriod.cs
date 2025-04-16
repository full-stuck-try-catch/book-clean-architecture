using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans;

public sealed record LoanPeriod
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    // Parameterless constructor for EF Core
    public LoanPeriod()
    {
    }

    public LoanPeriod(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<LoanPeriod> Create(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            return Result.Failure<LoanPeriod>(LoanErrors.LoanEndDateInvalid);
        }

        var loanPeriod = new LoanPeriod(startDate, endDate);

        return Result.Success(loanPeriod);
    }

    public bool IsOverdue(DateTime currentDate) => currentDate > EndDate;

    public bool IsActive(DateTime currentDate) => currentDate >= StartDate && currentDate <= EndDate;
}
