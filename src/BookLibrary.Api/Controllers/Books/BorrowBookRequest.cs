namespace BookLibrary.Api.Controllers.Books;

public sealed record BorrowBookRequest(
    DateTime StartDate,
    DateTime EndDate);
