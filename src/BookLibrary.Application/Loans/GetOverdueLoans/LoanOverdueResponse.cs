namespace BookLibrary.Application.Loans.GetOverdueLoans;

public sealed record LoanOverdueResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnedAt);
