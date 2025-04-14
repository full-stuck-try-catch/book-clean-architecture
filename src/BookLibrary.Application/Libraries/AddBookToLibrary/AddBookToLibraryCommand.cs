using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Libraries.AddBookToLibrary;

public sealed record AddBookToLibraryCommand(
    Guid LibraryId,
    string Title,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorCountry,
    int Quantity) : ICommand<Guid>;
