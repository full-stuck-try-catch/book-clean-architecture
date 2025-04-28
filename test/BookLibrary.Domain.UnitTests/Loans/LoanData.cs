using BookLibrary.Domain.Loans;

namespace BookLibrary.Domain.UnitTests.Loans;

internal static class LoanData
{
    public static Guid GetTestGuid() => Guid.NewGuid();

    public static Guid TestUserId => Guid.NewGuid();
    public static Guid TestBookId => Guid.NewGuid();

    public static LoanPeriod CreateLoanPeriod()
    {
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddDays(14);
        return LoanPeriod.Create(startDate, endDate).Value;
    }

    public static Loan CreateTestLoan()
    {
        return Loan.Create(
            GetTestGuid(),
            TestUserId,
            TestBookId,
            CreateLoanPeriod()).Value;
    }

    public static Loan CreateReturnedLoan()
    {
        Loan loan = CreateTestLoan();
        loan.MarkAsReturned(DateTime.UtcNow);
        loan.ClearDomainEvents();
        return loan;
    }
}
