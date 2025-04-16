namespace BookLibrary.Application.Loans.GetAllActiveLoans;

public sealed record LoanActiveResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnedAt);
