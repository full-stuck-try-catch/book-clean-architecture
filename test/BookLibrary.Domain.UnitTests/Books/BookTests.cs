using BookLibrary.Domain.Books;
using BookLibrary.Domain.UnitTests.Infrastructure;
using FluentAssertions;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books.Events;

namespace BookLibrary.Domain.UnitTests.Books;

public class BookTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Arrange
        Guid id = BookData.GetTestGuid();

        // Act
        var book = Book.Create(id, BookData.TestTitle, BookData.TestAuthor, BookData.TestQuantity, BookData.TestLibraryId);

        // Assert
        book.Id.Should().Be(id);
        book.Title.Should().Be(BookData.TestTitle);
        book.Author.Should().Be(BookData.TestAuthor);
        book.Quantity.Should().Be(BookData.TestQuantity);
        book.AvailableQuantity.Should().Be(BookData.TestQuantity);
        book.LibraryId.Should().Be(BookData.TestLibraryId);
        book.Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public void Create_Should_RaiseBookCreatedDomainEvent()
    {
        // Arrange
        Guid id = BookData.GetTestGuid();

        // Act
        var book = Book.Create(id, BookData.TestTitle, BookData.TestAuthor, BookData.TestQuantity, BookData.TestLibraryId);

        // Assert
        BookCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<BookCreatedDomainEvent>(book);
        domainEvent.Id.Should().Be(id);
        domainEvent.Title.Should().Be(BookData.TestTitle);
        domainEvent.Author.Should().Be(BookData.TestAuthor);
        domainEvent.Quantity.Should().Be(BookData.TestQuantity);
        domainEvent.LibraryId.Should().Be(BookData.TestLibraryId);
    }

    [Fact]
    public void MarkAsBorrowed_WithAvailableStock_Should_Succeed()
    {
        // Arrange
        Book book = BookData.CreateTestBook();
        int initialQuantity = book.Quantity;
        int initialAvailableQuantity = book.AvailableQuantity;

        // Act
        Result result = book.MarkAsBorrowed();

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Borrowed);
        book.Quantity.Should().Be(initialQuantity);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity - 1);
    }

    [Fact]
    public void MarkAsBorrowed_WithAvailableStock_Should_RaiseBookBorrowedDomainEvent()
    {
        // Arrange
        Book book = BookData.CreateTestBook();

        // Act
        book.MarkAsBorrowed();

        // Assert
        BookBorrowedDomainEvent domainEvent = AssertDomainEventWasPublished<BookBorrowedDomainEvent>(book);
        domainEvent.Book.Should().Be(book);
    }

    [Fact]
    public void MarkAsBorrowed_WithNoAvailableStock_Should_Fail()
    {
        // Arrange
        Book book = BookData.CreateBookWithNoAvailableStock();

        // Act
        Result result = book.MarkAsBorrowed();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookNotAvailable);
    }

    [Fact]
    public void MarkAsReturned_Should_Succeed()
    {
        // Arrange
        Book book = BookData.CreateBorrowedBook();
        int initialQuantity = book.Quantity;
        int initialAvailableQuantity = book.AvailableQuantity;

        // Act
        Result result = book.MarkAsReturned();

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Available);
        book.Quantity.Should().Be(initialQuantity);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity + 1);
    }

    [Fact]
    public void MarkAsReturned_Should_RaiseBookReturnedDomainEvent()
    {
        // Arrange
        Book book = BookData.CreateBorrowedBook();

        // Act
        book.MarkAsReturned();

        // Assert
        BookReturnedDomainEvent domainEvent = AssertDomainEventWasPublished<BookReturnedDomainEvent>(book);
        domainEvent.Book.Should().Be(book);
    }

    [Fact]
    public void AddStock_WithPositiveCount_Should_Succeed()
    {
        // Arrange
        Book book = BookData.CreateTestBook();
        int initialQuantity = book.Quantity;
        int initialAvailableQuantity = book.AvailableQuantity;
        const int stockToAdd = 5;

        // Act
        Result result = book.AddStock(stockToAdd);

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Quantity.Should().Be(initialQuantity + stockToAdd);
        book.AvailableQuantity.Should().Be(initialAvailableQuantity + stockToAdd);
    }

    [Fact]
    public void AddStock_WithZeroCount_Should_Fail()
    {
        // Arrange
        Book book = BookData.CreateTestBook();

        // Act
        Result result = book.AddStock(0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.InvalidStockCount);
    }

    [Fact]
    public void AddStock_WithNegativeCount_Should_Fail()
    {
        // Arrange
        Book book = BookData.CreateTestBook();

        // Act
        Result result = book.AddStock(-1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.InvalidStockCount);
    }

    [Fact]
    public void MarkAsDeleted_WithNoAvailableStock_Should_Succeed()
    {
        // Arrange
        Book book = BookData.CreateBookWithNoAvailableStock();

        // Act
        Result result = book.MarkAsDeleted();

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Deleted);
    }

    [Fact]
    public void MarkAsDeleted_WithAvailableStock_Should_Fail()
    {
        // Arrange
        Book book = BookData.CreateTestBook();

        // Act
        Result result = book.MarkAsDeleted();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookStillAvailable);
    }

    [Fact]
    public void MarkAsDeleted_AlreadyDeletedBook_Should_Fail()
    {
        // Arrange
        Book book = BookData.CreateDeletedBook();

        // Act
        Result result = book.MarkAsDeleted();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.BookAlreadyDeleted);
    }
}
