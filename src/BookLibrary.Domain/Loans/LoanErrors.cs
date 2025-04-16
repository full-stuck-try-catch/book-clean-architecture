using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans;

public static class LoanErrors
{
    public static readonly Error InvalidExtension = new("Loan.InvalidExtension", "The loan extension is invalid");
    public static readonly Error LoanAlreadyReturned = new("Loan.AlreadyReturned", "The loan is already returned");
    public static readonly Error NoBooksToLoan = new("Loan.NoBooksToLoan", "No books to loan");
    public static readonly Error LoanAlreadyExists = new("Loan.AlreadyExists", "A loan for this book already exists");
    public static readonly Error NotFound = new("Loan.NotFound", "The loan was not found");
    public static readonly Error LoanEndDateInvalid = new("Loan.EndDateInvalid", "The loan end date is invalid");
}
