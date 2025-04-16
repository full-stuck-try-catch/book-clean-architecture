namespace BookLibrary.Application.Loans.GetUserActiveLoans;

public sealed record LoanUserActiveResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnedAt);
