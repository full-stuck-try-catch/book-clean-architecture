using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans;

public static class LoanErrors
{
    public static readonly Error InvalidExtension = new("Loan.InvalidExtension", "The loan extension is invalid");
    public static readonly Error LoanAlreadyReturned = new("Loan.AlreadyReturned", "The loan is already returned");
    public static readonly Error NoBooksToLoan = new("Loan.NoBooksToLoan", "No books to loan");
}
