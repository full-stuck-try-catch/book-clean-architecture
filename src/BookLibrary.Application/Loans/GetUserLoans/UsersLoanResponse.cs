namespace BookLibrary.Application.Loans.GetUserLoans;

public sealed record UsersLoanResponse(
    Guid Id,
    Guid UserId,
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnedAt);
