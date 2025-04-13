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

    public static LoanPeriod Create(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        return new LoanPeriod(startDate, endDate);
    }

    public bool IsOverdue(DateTime currentDate) => currentDate > EndDate;

    public bool IsActive(DateTime currentDate) => currentDate >= StartDate && currentDate <= EndDate;
}
