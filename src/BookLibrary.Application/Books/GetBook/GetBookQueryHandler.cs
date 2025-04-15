using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Application.Books.GetBook;

internal sealed class GetBookQueryHandler : IQueryHandler<GetBookQuery, BookResponse>
{
    private readonly IBookRepository _bookRepository;

    public GetBookQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookResponse>> Handle(GetBookQuery request, CancellationToken cancellationToken)
    {
        // Find book
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure<BookResponse>(BookErrors.NotFound);
        }

        var response = new BookResponse(
            book.Id,
            book.Title.Value,
            book.Author.AuthorFirstName,
            book.Author.AuthorLastName,
            book.Author.AuthorCountry,
            book.LibraryId,
            book.Quantity,
            book.AvailableQuantity,
            book.Status.ToString(),
            book.IsAvailable);

        return Result.Success(response);
    }
}
