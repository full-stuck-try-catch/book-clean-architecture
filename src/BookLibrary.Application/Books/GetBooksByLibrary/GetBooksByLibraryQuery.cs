using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Books.GetBook;

namespace BookLibrary.Application.Books.GetBooksByLibrary;

public sealed record GetBooksByLibraryQuery(Guid LibraryId) : IQuery<IReadOnlyList<BookResponse>>;
