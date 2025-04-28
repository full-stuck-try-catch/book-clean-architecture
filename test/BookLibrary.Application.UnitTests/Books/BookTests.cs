using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Books.AddStock;
using BookLibrary.Application.Books.BorrowBook;
using BookLibrary.Application.Books.CreateBook;
using BookLibrary.Application.Books.GetBook;
using BookLibrary.Application.Books.GetBooksByLibrary;
using BookLibrary.Application.Books.MarkBookAsBorrowed;
using BookLibrary.Application.Books.MarkBookAsReturned;
using BookLibrary.Application.Books.ReturnBook;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using FluentAssertions;
using NSubstitute;

namespace BookLibrary.Application.UnitTests.Books;

public class BookTests
{
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    // Command handlers
    private readonly AddStockCommandHandler _addStockCommandHandler;
    private readonly BorrowBookCommandHandler _borrowBookCommandHandler;
    private readonly CreateBookCommandHandler _createBookCommandHandler;
    private readonly MarkBookAsBorrowedCommandHandler _markBookAsBorrowedCommandHandler;
    private readonly MarkBookAsReturnedCommandHandler _markBookAsReturnedCommandHandler;
    private readonly ReturnBookCommandHandler _returnBookCommandHandler;

    // Query handlers
    private readonly GetBookQueryHandler _getBookQueryHandler;
    private readonly GetBooksByLibraryQueryHandler _getBooksByLibraryQueryHandler;

    public BookTests()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _libraryRepository = Substitute.For<ILibraryRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _loanRepository = Substitute.For<ILoanRepository>();
        _userContext = Substitute.For<IUserContext>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        // Initialize command handlers
        _addStockCommandHandler = new AddStockCommandHandler(_bookRepository, _unitOfWork);
        _borrowBookCommandHandler = new BorrowBookCommandHandler(_userRepository, _bookRepository, _userContext, _loanRepository, _dateTimeProvider, _unitOfWork);
        _createBookCommandHandler = new CreateBookCommandHandler(_bookRepository, _libraryRepository, _unitOfWork);
        _markBookAsBorrowedCommandHandler = new MarkBookAsBorrowedCommandHandler(_bookRepository, _unitOfWork);
        _markBookAsReturnedCommandHandler = new MarkBookAsReturnedCommandHandler(_bookRepository, _unitOfWork);
        _returnBookCommandHandler = new ReturnBookCommandHandler(_userRepository, _bookRepository, _userContext, _dateTimeProvider, _unitOfWork);

        // Initialize query handlers
        _getBookQueryHandler = new GetBookQueryHandler(_bookRepository);
        _getBooksByLibraryQueryHandler = new GetBooksByLibraryQueryHandler(_bookRepository);
    }

    #region AddStock Tests

    [Fact]
    public async Task AddStock_ShouldReturnSuccess_WhenBookExistsAndCountIsValid()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = Book.Create(
            bookId,
            new BookTitle("Test Book"),
            new Author("John", "Doe", "USA"),
            5,
            Guid.NewGuid());

        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        Result result = await _addStockCommandHandler.Handle(new AddStockCommand(bookId, 3), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _bookRepository.Received(1).Update(book);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddStock_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result result = await _addStockCommandHandler.Handle(new AddStockCommand(bookId, 3), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region BorrowBook Tests

    [Fact]
    public async Task BorrowBook_ShouldReturnSuccess_WhenUserAndBookExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = DateTime.UtcNow.AddDays(14);

        var user = User.Create(
            userId,
            new Email("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        var book = Book.Create(
            bookId,
            new BookTitle("Test Book"),
            new Author("Jane", "Smith", "UK"),
            3,
            Guid.NewGuid());

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        Result result = await _borrowBookCommandHandler.Handle(new BorrowBookCommand(bookId, startDate, endDate), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = DateTime.UtcNow.AddDays(14);

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        Result result = await _borrowBookCommandHandler.Handle(new BorrowBookCommand(bookId, startDate, endDate), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = DateTime.UtcNow.AddDays(14);

        var user = User.Create(
            userId,
            new Email("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result result = await _borrowBookCommandHandler.Handle(new BorrowBookCommand(bookId, startDate, endDate), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region CreateBook Tests

    [Fact]
    public async Task CreateBook_ShouldReturnSuccess_WhenLibraryExistsAndBookIsNew()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(false);

        var command = new CreateBookCommand("Test Book", "John", "Doe", "USA", 5, libraryId);

        // Act
        Result<Guid> result = await _createBookCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _bookRepository.Received(1).Add(Arg.Any<Book>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBook_ShouldReturnFailure_WhenLibraryNotFound()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns((Library?)null);

        var command = new CreateBookCommand("Test Book", "John", "Doe", "USA", 5, libraryId);

        // Act
        Result<Guid> result = await _createBookCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.NotFound);
    }

    [Fact]
    public async Task CreateBook_ShouldReturnFailure_WhenQuantityIsInvalid()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);

        var command = new CreateBookCommand("Test Book", "John", "Doe", "USA", 0, libraryId);

        // Act
        Result<Guid> result = await _createBookCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.InvalidQuantity);
    }

    [Fact]
    public async Task CreateBook_ShouldReturnFailure_WhenBookAlreadyExists()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var library = Library.Create(libraryId, new LibraryName("Test Library"));

        _libraryRepository.GetByIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(library);
        _bookRepository.ExistsAsync(Arg.Any<BookTitle>(), Arg.Any<Author>(), Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateBookCommand("Test Book", "John", "Doe", "USA", 5, libraryId);

        // Act
        Result<Guid> result = await _createBookCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookAlreadyExists);
    }

    #endregion

    #region GetBook Tests

    [Fact]
    public async Task GetBook_ShouldReturnBookResponse_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var libraryId = Guid.NewGuid();
        var book = Book.Create(
            bookId,
            new BookTitle("Test Book"),
            new Author("John", "Doe", "USA"),
            5,
            libraryId);

        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        Result<BookResponse> result = await _getBookQueryHandler.Handle(new GetBookQuery(bookId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(bookId);
        result.Value.Title.Should().Be("Test Book");
        result.Value.AuthorFirstName.Should().Be("John");
        result.Value.AuthorLastName.Should().Be("Doe");
        result.Value.AuthorCountry.Should().Be("USA");
        result.Value.LibraryId.Should().Be(libraryId);
        result.Value.Quantity.Should().Be(5);
        result.Value.AvailableQuantity.Should().Be(5);
    }

    [Fact]
    public async Task GetBook_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result<BookResponse> result = await _getBookQueryHandler.Handle(new GetBookQuery(bookId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region GetBooksByLibrary Tests

    [Fact]
    public async Task GetBooksByLibrary_ShouldReturnBooksList_WhenBooksExist()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var books = new List<Book>
        {
            Book.Create(Guid.NewGuid(), new BookTitle("Book 1"), new Author("John", "Doe", "USA"), 3, libraryId),
            Book.Create(Guid.NewGuid(), new BookTitle("Book 2"), new Author("Jane", "Smith", "UK"), 2, libraryId)
        };

        _bookRepository.GetByLibraryIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(books);

        // Act
        Result<IReadOnlyList<BookResponse>> result = await _getBooksByLibraryQueryHandler.Handle(
            new GetBooksByLibraryQuery(libraryId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Title.Should().Be("Book 1");
        result.Value[1].Title.Should().Be("Book 2");
    }

    [Fact]
    public async Task GetBooksByLibrary_ShouldReturnEmptyList_WhenNoBooksExist()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        _bookRepository.GetByLibraryIdAsync(libraryId, Arg.Any<CancellationToken>()).Returns(new List<Book>());

        // Act
        Result<IReadOnlyList<BookResponse>> result = await _getBooksByLibraryQueryHandler.Handle(
            new GetBooksByLibraryQuery(libraryId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion

    #region MarkBookAsBorrowed Tests

    [Fact]
    public async Task MarkBookAsBorrowed_ShouldReturnSuccess_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = Book.Create(
            bookId,
            new BookTitle("Test Book"),
            new Author("John", "Doe", "USA"),
            5,
            Guid.NewGuid());

        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        Result result = await _markBookAsBorrowedCommandHandler.Handle(
            new MarkBookAsBorrowedCommand(bookId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _bookRepository.Received(1).Update(book);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkBookAsBorrowed_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result result = await _markBookAsBorrowedCommandHandler.Handle(
            new MarkBookAsBorrowedCommand(bookId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region MarkBookAsReturned Tests

    [Fact]
    public async Task MarkBookAsReturned_ShouldReturnSuccess_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = Book.Create(
            bookId,
            new BookTitle("Test Book"),
            new Author("John", "Doe", "USA"),
            5,
            Guid.NewGuid());

        // First mark it as borrowed so it can be returned
        book.MarkAsBorrowed();

        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        Result result = await _markBookAsReturnedCommandHandler.Handle(
            new MarkBookAsReturnedCommand(bookId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _bookRepository.Received(1).Update(book);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkBookAsReturned_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result result = await _markBookAsReturnedCommandHandler.Handle(
            new MarkBookAsReturnedCommand(bookId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region ReturnBook Tests

    [Fact]
    public async Task ReturnBook_ShouldReturnSuccess_WhenUserAndBookExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var user = User.Create(
            userId,
            new Email("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        Result<LoanPeriod> loanPerior = LoanPeriod.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        Result<Loan> loan = Loan.Create(
                Guid.NewGuid(),
                userId,
                bookId,
                loanPerior.Value);

        var book = Book.Create(
                bookId,
                new BookTitle("Test Book"),
                new Author("Jane", "Smith", "UK"),
                3,
                Guid.NewGuid());

        user.BorrowBook(book , loan.Value);

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns(book);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        Result result = await _returnBookCommandHandler.Handle(new ReturnBookCommand(bookId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnBook_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        Result result = await _returnBookCommandHandler.Handle(new ReturnBookCommand(bookId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task ReturnBook_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var user = User.Create(
            userId,
            new Email("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _bookRepository.GetByIdAsync(bookId, Arg.Any<CancellationToken>()).Returns((Book?)null);

        // Act
        Result result = await _returnBookCommandHandler.Handle(new ReturnBookCommand(bookId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion
}
