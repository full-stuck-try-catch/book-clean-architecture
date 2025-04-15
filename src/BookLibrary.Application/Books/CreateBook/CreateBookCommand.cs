using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorCountry,
    int Quantity,
    Guid LibraryId) : ICommand<Guid>;
