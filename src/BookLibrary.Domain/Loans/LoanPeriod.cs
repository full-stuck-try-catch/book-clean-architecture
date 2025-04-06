namespace BookLibrary.Domain.Loans;

public sealed record LoanPeriod(
    DateTime StartDate,
    DateTime EndDate)
{
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

    public static LoanPeriod operator +(LoanPeriod period, TimeSpan extension) =>
        new LoanPeriod(period.StartDate, period.EndDate.Add(extension));
}
