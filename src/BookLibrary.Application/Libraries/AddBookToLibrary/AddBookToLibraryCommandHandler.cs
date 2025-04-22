using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;

namespace BookLibrary.Application.Libraries.AddBookToLibrary;

public sealed class AddBookToLibraryCommandHandler : ICommandHandler<AddBookToLibraryCommand, Guid>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBookToLibraryCommandHandler(
        ILibraryRepository libraryRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _libraryRepository = libraryRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddBookToLibraryCommand request, CancellationToken cancellationToken)
    {
        // Find library
        Library? library = await _libraryRepository.GetByIdAsync(request.LibraryId, cancellationToken);
        if (library is null)
        {
            return Result.Failure<Guid>(LibraryErrors.NotFound);
        }

        // Create value objects
        var bookTitle = new BookTitle(request.Title);
        var author = new Author(request.AuthorFirstName, request.AuthorLastName, request.AuthorCountry);

        // Check if book already exists in the library
        bool bookExists = await _bookRepository.ExistsAsync(bookTitle, author, cancellationToken);
        if (bookExists)
        {
            return Result.Failure<Guid>(LibraryErrors.BookAlreadyExists);
        }

        // Create book
        var book = Book.Create(
            Guid.NewGuid(),
            bookTitle,
            author,
            request.Quantity,
            library.Id);

        // Add book to library (domain logic)
        Result addResult = library.AddBook(book);
        if (addResult.IsFailure)
        {
            return Result.Failure<Guid>(addResult.Error);
        }

        // Add book to repository
        _bookRepository.Add(book);

        // Update library
        _libraryRepository.Update(library);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id);
    }
}
