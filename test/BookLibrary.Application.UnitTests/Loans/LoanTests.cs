using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Loans.CreateLoan;
using BookLibrary.Application.Loans.ExtendLoan;
using BookLibrary.Application.Loans.GetLoan;
using BookLibrary.Application.Loans.GetUserLoans;
using BookLibrary.Application.Loans.ReturnLoan;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using FluentAssertions;
using NSubstitute;

namespace BookLibrary.Application.UnitTests.Loans;

public class LoanTests
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    // Command handlers
    private readonly CreateLoanCommandHandler _createLoanCommandHandler;
    private readonly ExtendLoanCommandHandler _extendLoanCommandHandler;
    private readonly ReturnLoanCommandHandler _returnLoanCommandHandler;

    // Query handlers
    private readonly GetLoanQueryHandler _getLoanQueryHandler;
    private readonly GetUserLoansQueryHandler _getUserLoansQueryHandler;

    // Test data
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _bookId = Guid.NewGuid();
    private readonly Guid _loanId = Guid.NewGuid();
    private readonly DateTime _startDate = DateTime.UtcNow;
    private readonly DateTime _endDate = DateTime.UtcNow.AddDays(14);

    public LoanTests()
    {
        _loanRepository = Substitute.For<ILoanRepository>();
        _bookRepository = Substitute.For<IBookRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _userContext = Substitute.For<IUserContext>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        // Initialize command handlers
        _createLoanCommandHandler = new CreateLoanCommandHandler(
            _loanRepository,
            _userRepository,
            _bookRepository,
            _userContext,
            _unitOfWork);

        _extendLoanCommandHandler = new ExtendLoanCommandHandler(
            _loanRepository,
            _userContext,
            _unitOfWork);

        _returnLoanCommandHandler = new ReturnLoanCommandHandler(
            _loanRepository,
            _bookRepository,
            _userContext,
            _unitOfWork);

        // Initialize query handlers
        _getLoanQueryHandler = new GetLoanQueryHandler(
            _loanRepository,
            _userContext);

        _getUserLoansQueryHandler = new GetUserLoansQueryHandler(
            _loanRepository,
            _userContext);

        // Setup user context
        _userContext.UserId.Returns(_userId);
    }

    #region CreateLoanCommandHandler Tests

    [Fact]
    public async Task CreateLoan_WithValidData_Should_Succeed()
    {
        // Arrange
        var command = new CreateLoanCommand(_bookId, _startDate, _endDate);
        User user = CreateTestUser(_userId);
        Book book = CreateAvailableBook(_bookId);

        _userRepository.GetByIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _loanRepository.GetActiveLoanByUserAndBookAsync(_userId, _bookId, Arg.Any<CancellationToken>())
            .Returns((Loan?)null);

        // Act
        Result<Guid> result = await _createLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _loanRepository.Received(1).Add(Arg.Any<Loan>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        // Verify book was marked as borrowed by checking its status
        book.Status.Should().Be(BookStatus.Borrowed);
    }

    [Fact]
    public async Task CreateLoan_WithNonExistentUser_Should_ReturnUserNotFoundError()
    {
        // Arrange
        var command = new CreateLoanCommand(_bookId, _startDate, _endDate);

        _userRepository.GetByIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        Result<Guid> result = await _createLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotFound);
        _loanRepository.DidNotReceive().Add(Arg.Any<Loan>());
    }

    [Fact]
    public async Task CreateLoan_WithNonExistentBook_Should_ReturnBookNotFoundError()
    {
        // Arrange
        var command = new CreateLoanCommand(_bookId, _startDate, _endDate);
        User user = CreateTestUser(_userId);

        _userRepository.GetByIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        Result<Guid> result = await _createLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
        _loanRepository.DidNotReceive().Add(Arg.Any<Loan>());
    }

    [Fact]
    public async Task CreateLoan_WithUnavailableBook_Should_ReturnBookNotAvailableError()
    {
        // Arrange
        var command = new CreateLoanCommand(_bookId, _startDate, _endDate);
        User user = CreateTestUser(_userId);
        Book book = CreateUnavailableBook(_bookId);

        _userRepository.GetByIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);

        // Act
        Result<Guid> result = await _createLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookNotAvailable);
        _loanRepository.DidNotReceive().Add(Arg.Any<Loan>());
    }

    [Fact]
    public async Task CreateLoan_WithExistingActiveLoan_Should_ReturnLoanAlreadyExistsError()
    {
        // Arrange
        var command = new CreateLoanCommand(_bookId, _startDate, _endDate);
        User user = CreateTestUser(_userId);
        Book book = CreateAvailableBook(_bookId);
        Loan existingLoan = CreateTestLoan(_loanId, _userId, _bookId);

        _userRepository.GetByIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _loanRepository.GetActiveLoanByUserAndBookAsync(_userId, _bookId, Arg.Any<CancellationToken>())
            .Returns(existingLoan);

        // Act
        Result<Guid> result = await _createLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.LoanAlreadyExists);
        _loanRepository.DidNotReceive().Add(Arg.Any<Loan>());
    }

    #endregion

    #region ExtendLoanCommandHandler Tests

    [Fact]
    public async Task ExtendLoan_WithValidData_Should_Succeed()
    {
        // Arrange
        DateTime newEndDate = _endDate.AddDays(7);
        var command = new ExtendLoanCommand(_loanId, newEndDate);
        Loan loan = CreateTestLoan(_loanId, _userId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);

        // Act
        Result result = await _extendLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtendLoan_WithNonExistentLoan_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        DateTime newEndDate = _endDate.AddDays(7);
        var command = new ExtendLoanCommand(_loanId, newEndDate);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns((Loan?)null);

        // Act
        Result result = await _extendLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtendLoan_WithLoanNotBelongingToUser_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        DateTime newEndDate = _endDate.AddDays(7);
        var command = new ExtendLoanCommand(_loanId, newEndDate);
        var otherUserId = Guid.NewGuid();
        Loan loan = CreateTestLoan(_loanId, otherUserId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);

        // Act
        Result result = await _extendLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region ReturnLoanCommandHandler Tests

    [Fact]
    public async Task ReturnLoan_WithValidData_Should_Succeed()
    {
        // Arrange
        DateTime returnedAt = DateTime.UtcNow;
        var command = new ReturnLoanCommand(_loanId, returnedAt);
        Loan loan = CreateTestLoan(_loanId, _userId, _bookId);
        Book book = CreateAvailableBook(_bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);

        // Act
        Result result = await _returnLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        // Verify book was marked as returned by checking its status
        book.Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public async Task ReturnLoan_WithNonExistentLoan_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        DateTime returnedAt = DateTime.UtcNow;
        var command = new ReturnLoanCommand(_loanId, returnedAt);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns((Loan?)null);

        // Act
        Result result = await _returnLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnLoan_WithLoanNotBelongingToUser_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        DateTime returnedAt = DateTime.UtcNow;
        var command = new ReturnLoanCommand(_loanId, returnedAt);
        var otherUserId = Guid.NewGuid();
        Loan loan = CreateTestLoan(_loanId, otherUserId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);

        // Act
        Result result = await _returnLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnLoan_WithNonExistentBook_Should_ReturnBookNotFoundError()
    {
        // Arrange
        DateTime returnedAt = DateTime.UtcNow;
        var command = new ReturnLoanCommand(_loanId, returnedAt);
        Loan loan = CreateTestLoan(_loanId, _userId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);
        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        Result result = await _returnLoanCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetLoanQueryHandler Tests

    [Fact]
    public async Task GetLoan_WithValidData_Should_ReturnLoanResponse()
    {
        // Arrange
        var query = new GetLoanQuery(_loanId);
        Loan loan = CreateTestLoan(_loanId, _userId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);

        // Act
        Result<LoanResponse> result = await _getLoanQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_loanId);
        result.Value.UserId.Should().Be(_userId);
        result.Value.BookId.Should().Be(_bookId);
        result.Value.StartDate.Should().Be(loan.Period.StartDate);
        result.Value.EndDate.Should().Be(loan.Period.EndDate);
        result.Value.ReturnedAt.Should().Be(loan.ReturnedAt);
    }

    [Fact]
    public async Task GetLoan_WithNonExistentLoan_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        var query = new GetLoanQuery(_loanId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns((Loan?)null);

        // Act
        Result<LoanResponse> result = await _getLoanQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
    }

    [Fact]
    public async Task GetLoan_WithLoanNotBelongingToUser_Should_ReturnLoanNotFoundError()
    {
        // Arrange
        var query = new GetLoanQuery(_loanId);
        var otherUserId = Guid.NewGuid();
        Loan loan = CreateTestLoan(_loanId, otherUserId, _bookId);

        _loanRepository.GetByIdAsync(_loanId, Arg.Any<CancellationToken>())
            .Returns(loan);

        // Act
        Result<LoanResponse> result = await _getLoanQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.NotFound);
    }

    #endregion

    #region GetUserLoansQueryHandler Tests

    [Fact]
    public async Task GetUserLoans_WithValidData_Should_ReturnUserLoansResponse()
    {
        // Arrange
        var query = new GetUserLoansQuery();
        Loan loan1 = CreateTestLoan(Guid.NewGuid(), _userId, _bookId);
        Loan loan2 = CreateTestLoan(Guid.NewGuid(), _userId, Guid.NewGuid());
        var loans = new List<Loan> { loan1, loan2 };

        _loanRepository.GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(loans);

        // Act
        Result<List<UsersLoanResponse>> result = await _getUserLoansQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        
        UsersLoanResponse firstLoan = result.Value.First(l => l.Id == loan1.Id);
        firstLoan.UserId.Should().Be(_userId);
        firstLoan.BookId.Should().Be(loan1.BookId);
        firstLoan.StartDate.Should().Be(loan1.Period.StartDate);
        firstLoan.EndDate.Should().Be(loan1.Period.EndDate);
        firstLoan.ReturnedAt.Should().Be(loan1.ReturnedAt);
    }

    [Fact]
    public async Task GetUserLoans_WithNoLoans_Should_ReturnEmptyList()
    {
        // Arrange
        var query = new GetUserLoansQuery();
        var loans = new List<Loan>();

        _loanRepository.GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(loans);

        // Act
        Result<List<UsersLoanResponse>> result = await _getUserLoansQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static User CreateTestUser(Guid userId)
    {
        var email = new Email("test@example.com");
        var firstName = new FirstName("Test");
        var lastName = new LastName("User");
        var passwordHash = new PasswordHash("hashedpassword");

        return User.Create(userId, email, firstName, lastName, passwordHash, DateTime.UtcNow);
    }

    private static Book CreateAvailableBook(Guid bookId)
    {
        var bookTitle = new BookTitle("Test Book");
        var author = new Author("John", "Doe", "USA");
        var book = Book.Create(bookId, bookTitle, author, 1, Guid.NewGuid());
        
        // Clear domain events to avoid side effects in tests
        book.ClearDomainEvents();
        
        return book;
    }

    private static Book CreateUnavailableBook(Guid bookId)
    {
        var bookTitle = new BookTitle("Unavailable Book");
        var author = new Author("Jane", "Smith", "UK");
        var book = Book.Create(bookId, bookTitle, author, 1, Guid.NewGuid());
        
        // Mark as borrowed to make it unavailable
        book.MarkAsBorrowed();
        
        // Clear domain events to avoid side effects in tests
        book.ClearDomainEvents();
        
        return book;
    }

    private static Loan CreateTestLoan(Guid loanId, Guid userId, Guid bookId)
    {
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddDays(14);
        var loanPeriod = new LoanPeriod(startDate, endDate);
        
        Loan loan = Loan.Create(loanId, userId, bookId, loanPeriod).Value;
        
        // Clear domain events to avoid side effects in tests
        loan.ClearDomainEvents();
        
        return loan;
    }

    #endregion
}
