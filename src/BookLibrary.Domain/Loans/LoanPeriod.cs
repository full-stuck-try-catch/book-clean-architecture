namespace BookLibrary.Domain.Loans;

public sealed record LoanPeriod(
    DateTime StartDate,
    DateTime? ReturnDate,
    DateTime EndDate)
{
    public static LoanPeriod Create(DateTime startDate, DateTime? returnDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        if (returnDate.HasValue && returnDate < startDate)
        {
            throw new ArgumentException("Return date must be after start date.");
        }

        return new LoanPeriod(startDate, returnDate, endDate);
    }

    public bool IsOverdue(DateTime currentDate) => currentDate > EndDate;

    public bool IsActive(DateTime currentDate) => currentDate >= StartDate && currentDate <= EndDate;

    public static LoanPeriod operator +(LoanPeriod period, TimeSpan extension) =>
        new LoanPeriod(DateTime.UtcNow, null, period.EndDate.Add(extension));
}
