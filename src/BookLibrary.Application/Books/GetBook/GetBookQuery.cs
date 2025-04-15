using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.GetBook;

public sealed record GetBookQuery(Guid BookId) : IQuery<BookResponse>;
