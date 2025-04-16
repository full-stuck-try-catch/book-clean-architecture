namespace BookLibrary.Application.Loans.GetLoan;

public sealed record LoanResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnedAt);
