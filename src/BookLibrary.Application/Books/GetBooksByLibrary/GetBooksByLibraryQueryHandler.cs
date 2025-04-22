using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Books.GetBook;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Application.Books.GetBooksByLibrary;

public sealed class GetBooksByLibraryQueryHandler : IQueryHandler<GetBooksByLibraryQuery, IReadOnlyList<BookResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksByLibraryQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<IReadOnlyList<BookResponse>>> Handle(GetBooksByLibraryQuery request, CancellationToken cancellationToken)
    {
        // Get all books for the specified library
        List<Book> books = await _bookRepository.GetByLibraryIdAsync(request.LibraryId, cancellationToken);

        var response = books.Select(book => new BookResponse(
            book.Id,
            book.Title.Value,
            book.Author.AuthorFirstName,
            book.Author.AuthorLastName,
            book.Author.AuthorCountry,
            book.LibraryId,
            book.Quantity,
            book.AvailableQuantity,
            book.Status.ToString()))
            .ToList();

        return Result.Success<IReadOnlyList<BookResponse>>(response);
    }
}
