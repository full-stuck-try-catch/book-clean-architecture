using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;

namespace BookLibrary.Application.Books.CreateBook;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(
        IBookRepository bookRepository,
        ILibraryRepository libraryRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _libraryRepository = libraryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Validate library exists
        Library? library = await _libraryRepository.GetByIdAsync(request.LibraryId, cancellationToken);
        if (library is null)
        {
            return Result.Failure<Guid>(LibraryErrors.NotFound);
        }

        // Validate quantity
        if (request.Quantity <= 0)
        {
            return Result.Failure<Guid>(BookErrors.InvalidQuantity);
        }

        // Create value objects
        var bookTitle = new BookTitle(request.Title);
        var author = new Author(request.AuthorFirstName, request.AuthorLastName, request.AuthorCountry);

        // Check if book already exists
        bool bookExists = await _bookRepository.ExistsAsync(bookTitle, author, cancellationToken);
        if (bookExists)
        {
            return Result.Failure<Guid>(BookErrors.BookAlreadyExists);
        }

        // Create book
        var book = Book.Create(
            Guid.NewGuid(),
            bookTitle,
            author,
            request.Quantity,
            request.LibraryId);

        // Add to repository
        _bookRepository.Add(book);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id);
    }
}
