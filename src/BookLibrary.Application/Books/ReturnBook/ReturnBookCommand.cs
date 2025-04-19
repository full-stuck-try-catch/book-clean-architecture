using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.ReturnBook;

public sealed record ReturnBookCommand(Guid BookId) : ICommand;
