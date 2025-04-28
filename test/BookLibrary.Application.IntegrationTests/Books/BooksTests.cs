using Bookify.Application.IntegrationTests.Infrastructure;
using BookLibrary.Application.Books.AddStock;
using BookLibrary.Application.Books.CreateBook;
using BookLibrary.Application.Books.MarkBookAsBorrowed;
using BookLibrary.Application.Books.MarkBookAsDeleted;
using BookLibrary.Application.Books.MarkBookAsReturned;
using BookLibrary.Application.Exceptions;
using BookLibrary.Application.IntegrationTests.Infrastructure;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Application.IntegrationTests.Books;

public class BooksTests : BaseIntegrationTest
{
    public BooksTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    #region CreateBookCommand Tests

    [Fact]
    public async Task CreateBookCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        Library library = await CreateTestLibraryAsync();
        
        var command = new CreateBookCommand(
            "Test Book Title",
            "John",
            "Doe", 
            "USA",
            5,
            library.Id);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        Book? book = await DbContext.Books.FirstOrDefaultAsync(b => b.Id == result.Value);
        book.Should().NotBeNull();
        book!.Title.Value.Should().Be("Test Book Title");
        book.Author.AuthorFirstName.Should().Be("John");
        book.Author.AuthorLastName.Should().Be("Doe");
        book.Author.AuthorCountry.Should().Be("USA");
        book.Quantity.Should().Be(5);
        book.AvailableQuantity.Should().Be(5);
        book.Status.Should().Be(BookStatus.Available);
        book.LibraryId.Should().Be(library.Id);
    }

    [Fact]
    public async Task CreateBookCommand_WithNonExistentLibrary_ShouldFail()
    {
        // Arrange
        var command = new CreateBookCommand(
            "Test Book",
            "John", 
            "Doe",
            "USA",
            5,
            Guid.NewGuid());

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.NotFound);
    }

    #endregion

    #region AddStockCommand Tests

    [Fact]
    public async Task AddStockCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        Book book = await CreateTestBookAsync();
        int initialQuantity = book.Quantity;
        int initialAvailableQuantity = book.AvailableQuantity;
        
        var command = new AddStockCommand(book.Id, 3);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await DbContext.Entry(book).ReloadAsync();
        book.Quantity.Should().Be(initialQuantity + 3);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity + 3);
    }

    [Fact]
    public async Task AddStockCommand_WithInvalidCount_ShouldFail()
    {
        // Arrange
        Book book = await CreateTestBookAsync();
        var command = new AddStockCommand(book.Id, 0);

        // Act & Assert
        ValidationException exception = await Assert.ThrowsAsync<ValidationException>(
            () => Sender.Send(command));

        exception.Errors.Should().HaveCount(1);
        ValidationError error = exception.Errors.First();
        error.PropertyName.Should().Be("Count");
        error.ErrorMessage.Should().Be("Stock count must be greater than zero.");
    }

    [Fact]
    public async Task AddStockCommand_WithNonExistentBook_ShouldFail()
    {
        // Arrange
        var command = new AddStockCommand(Guid.NewGuid(), 5);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region ReturnBookCommand Tests

    #endregion

    #region MarkBookAsBorrowedCommand Tests

    [Fact]
    public async Task MarkBookAsBorrowedCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        Book book = await CreateTestBookAsync();
        int initialAvailableQuantity = book.AvailableQuantity;
        
        var command = new MarkBookAsBorrowedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await DbContext.Entry(book).ReloadAsync();
        book.Status.Should().Be(BookStatus.Borrowed);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity - 1);
    }

    [Fact]
    public async Task MarkBookAsBorrowedCommand_WithNoAvailableCopies_ShouldFail()
    {
        // Arrange
        Book book = await CreateTestBookAsync(availableQuantity: 0);
        var command = new MarkBookAsBorrowedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookNotAvailable);
    }

    [Fact]
    public async Task MarkBookAsBorrowedCommand_WithNonExistentBook_ShouldFail()
    {
        // Arrange
        var command = new MarkBookAsBorrowedCommand(Guid.NewGuid());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region MarkBookAsReturnedCommand Tests

    [Fact]
    public async Task MarkBookAsReturnedCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        Book book = await CreateTestBookAsync();
        // First borrow the book
        book.MarkAsBorrowed();
        await DbContext.SaveChangesAsync();
        
        int initialAvailableQuantity = book.AvailableQuantity;
        var command = new MarkBookAsReturnedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await DbContext.Entry(book).ReloadAsync();
        book.Status.Should().Be(BookStatus.Available);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity + 1);
    }

    [Fact]
    public async Task MarkBookAsReturnedCommand_WithNonExistentBook_ShouldFail()
    {
        // Arrange
        var command = new MarkBookAsReturnedCommand(Guid.NewGuid());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();  
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region MarkBookAsDeletedCommand Tests

    [Fact]
    public async Task MarkBookAsDeletedCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        Book book = await CreateTestBookAsync(availableQuantity: 0); // No available copies
        var command = new MarkBookAsDeletedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await DbContext.Entry(book).ReloadAsync();
        book.Status.Should().Be(BookStatus.Deleted);
    }

    [Fact]
    public async Task MarkBookAsDeletedCommand_WithAvailableCopies_ShouldFail()
    {
        // Arrange
        Book book = await CreateTestBookAsync(availableQuantity: 1);
        var command = new MarkBookAsDeletedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookStillAvailable);
    }

    [Fact]
    public async Task MarkBookAsDeletedCommand_WithAlreadyDeletedBook_ShouldFail()
    {
        // Arrange
        Book book = await CreateTestBookAsync(availableQuantity: 0);
        book.MarkAsDeleted();
        await DbContext.SaveChangesAsync();
        
        var command = new MarkBookAsDeletedCommand(book.Id);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookAlreadyDeleted);
    }

    [Fact]
    public async Task MarkBookAsDeletedCommand_WithNonExistentBook_ShouldFail()
    {
        // Arrange
        var command = new MarkBookAsDeletedCommand(Guid.NewGuid());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task<Library> CreateTestLibraryAsync()
    {
        LibraryName libraryName = new("Test Library");
        var library = Library.Create(Guid.NewGuid(), libraryName);
        
        DbContext.Libraries.Add(library);
        await DbContext.SaveChangesAsync();
        
        return library;
    }

    private async Task<Book> CreateTestBookAsync(int quantity = 5, int? availableQuantity = null)
    {
        Library library = await CreateTestLibraryAsync();
        
        BookTitle bookTitle = new("Test Book");
        Author author = new("John", "Doe", "USA");
        var book = Book.Create(Guid.NewGuid(), bookTitle, author, quantity, library.Id);
        
        // Manually set available quantity if specified
        if (availableQuantity.HasValue && availableQuantity != quantity)
        {
            int difference = quantity - availableQuantity.Value;
            for (int i = 0; i < difference; i++)
            {
                book.MarkAsBorrowed();
            }
        }
        
        DbContext.Books.Add(book);
        await DbContext.SaveChangesAsync();
        
        return book;
    }

    private async Task<User> CreateTestUserAsync()
    {
        var email = new Email("test@example.com");
        FirstName firstName = new("Test");
        LastName lastName = new("User");
        PasswordHash passwordHash = new("hashedPassword");
        
        var user = User.Create(
            Guid.NewGuid(),
            email,
            firstName,
            lastName,
            passwordHash,
            DateTime.UtcNow);
        
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        
        return user;
    }

    private async Task<Loan> CreateTestLoanAsync(Guid bookId, Guid userId)
    {
        LoanPeriod period = new(DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        Result<Loan> loanResult = Loan.Create(
            Guid.NewGuid(),
            userId,
            bookId,
            period);
        
        DbContext.Loans.Add(loanResult.Value);
        await DbContext.SaveChangesAsync();
        
        return loanResult.Value;
    }

    #endregion
}
