using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.BorrowBook;

public sealed record BorrowBookCommand(
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate) : ICommand;
