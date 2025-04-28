using BookLibrary.Application.Libraries.AddBookToLibrary;
using BookLibrary.Application.Libraries.CreateLibrary;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace BookLibrary.Application.UnitTests.Libraries;

public class LibraryTests
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Command handlers
    private readonly CreateLibraryCommandHandler _createLibraryCommandHandler;
    private readonly AddBookToLibraryCommandHandler _addBookToLibraryCommandHandler;

    public LibraryTests()
    {
        _libraryRepository = Substitute.For<ILibraryRepository>();
        _bookRepository = Substitute.For<IBookRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        // Initialize command handlers
        _createLibraryCommandHandler = new CreateLibraryCommandHandler(_libraryRepository, _unitOfWork);
        _addBookToLibraryCommandHandler = new AddBookToLibraryCommandHandler(_libraryRepository, _bookRepository, _unitOfWork);
    }

    #region CreateLibrary Tests

    [Fact]
    public async Task CreateLibrary_ShouldReturnSuccess_WhenLibraryNameIsValidAndUnique()
    {
        // Arrange
        var libraryName = new LibraryName("Test Library");
        _libraryRepository.ExistsAsync(libraryName, Arg.Any<CancellationToken>()).Returns(false);

        var command = new CreateLibraryCommand(libraryName);

        // Act
        Result<Guid> result = await _createLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _libraryRepository.Received(1).Add(Arg.Any<Library>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLibrary_ShouldReturnFailure_WhenLibraryNameIsEmpty()
    {
        // Arrange
        var command = new CreateLibraryCommand(new LibraryName(""));

        // Act
        Result<Guid> result = await _createLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.InvalidName);
        _libraryRepository.DidNotReceive().Add(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLibrary_ShouldReturnFailure_WhenLibraryNameIsWhitespace()
    {
        // Arrange
        var command = new CreateLibraryCommand(new LibraryName("   "));

        // Act
        Result<Guid> result = await _createLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.InvalidName);
        _libraryRepository.DidNotReceive().Add(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLibrary_ShouldReturnFailure_WhenLibraryNameIsNull()
    {
        // Arrange
        var command = new CreateLibraryCommand(new LibraryName(null!));

        // Act
        Result<Guid> result = await _createLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.InvalidName);
        _libraryRepository.DidNotReceive().Add(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLibrary_ShouldReturnFailure_WhenLibraryNameAlreadyExists()
    {
        // Arrange
        var libraryName = new LibraryName("Existing Library");

        _libraryRepository.ExistsAsync(libraryName, Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateLibraryCommand(libraryName);

        // Act
        Result<Guid> result = await _createLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.NameAlreadyExists);
        _libraryRepository.DidNotReceive().Add(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region AddBookToLibrary Tests

    [Fact]
    public async Task AddBookToLibrary_ShouldReturnSuccess_WhenLibraryExistsAndBookIsNew()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(false);

        var command = new AddBookToLibraryCommand(libraryId, "Test Book", "John", "Doe", "USA", 5);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _bookRepository.Received(1).Add(Arg.Any<Book>());
        _libraryRepository.Received(1).Update(library);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddBookToLibrary_ShouldReturnFailure_WhenLibraryNotFound()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns((Library?)null);

        var command = new AddBookToLibraryCommand(libraryId, "Test Book", "John", "Doe", "USA", 5);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.NotFound);
        _bookRepository.DidNotReceive().Add(Arg.Any<Book>());
        _libraryRepository.DidNotReceive().Update(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddBookToLibrary_ShouldReturnFailure_WhenBookAlreadyExists()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(true);

        var command = new AddBookToLibraryCommand(libraryId, "Existing Book", "Jane", "Smith", "UK", 3);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.BookAlreadyExists);
        _bookRepository.DidNotReceive().Add(Arg.Any<Book>());
        _libraryRepository.DidNotReceive().Update(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddBookToLibrary_ShouldReturnFailure_WhenAddBookToLibraryDomainFails()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));
        
        // Create a book that already exists in the library to cause domain failure
        var existingBook = Book.Create(
            Guid.NewGuid(),
            new BookTitle("Test Book"),
            new Author("John", "Doe", "USA"),
            5,
            libraryId);
        
        library.AddBook(existingBook); // Add the book to make it exist in the library

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(false);

        var command = new AddBookToLibraryCommand(libraryId, "Test Book", "John", "Doe", "USA", 5);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.BookAlreadyExists);
        _bookRepository.DidNotReceive().Add(Arg.Any<Book>());
        _libraryRepository.DidNotReceive().Update(Arg.Any<Library>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddBookToLibrary_ShouldCreateCorrectBookWithProperValues()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));
        Book? capturedBook = null;

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(false);
        _bookRepository.When(x => x.Add(Arg.Any<Book>())).Do(x => capturedBook = x.Arg<Book>());

        var command = new AddBookToLibraryCommand(libraryId, "Test Book", "John", "Doe", "USA", 10);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedBook.Should().NotBeNull();
        capturedBook!.Title.Value.Should().Be("Test Book");
        capturedBook.Author.AuthorFirstName.Should().Be("John");
        capturedBook.Author.AuthorLastName.Should().Be("Doe");
        capturedBook.Author.AuthorCountry.Should().Be("USA");
        capturedBook.Quantity.Should().Be(10);
        capturedBook.LibraryId.Should().Be(libraryId);
    }

    [Fact]
    public async Task AddBookToLibrary_ShouldReturnBookId_WhenSuccessful()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(false);

        var command = new AddBookToLibraryCommand(libraryId, "Test Book", "John", "Doe", "USA", 5);

        // Act
        Result<Guid> result = await _addBookToLibraryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        result.Value.Should().NotBe(libraryId); // Should be a different GUID (the book ID)
    }

    #endregion
}
