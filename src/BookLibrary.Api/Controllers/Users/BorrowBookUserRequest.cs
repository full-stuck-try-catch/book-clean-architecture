namespace BookLibrary.Api.Controllers.Users;

public sealed record BorrowBookUserRequest(
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate);
