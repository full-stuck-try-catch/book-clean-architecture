using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.MarkBookAsBorrowed;

public sealed record MarkBookAsBorrowedCommand(Guid BookId) : ICommand;
