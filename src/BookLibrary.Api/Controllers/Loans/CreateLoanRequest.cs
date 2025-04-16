namespace BookLibrary.Api.Controllers.Loans;

public sealed record CreateLoanRequest(
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate);
