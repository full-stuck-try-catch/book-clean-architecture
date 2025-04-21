using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Libraries.Events;
using BookLibrary.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace BookLibrary.Domain.UnitTests.Libraries;

public class LibraryTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Arrange
        Guid id = LibraryData.GetTestGuid();
        LibraryName name = LibraryData.TestLibraryName;

        // Act
        var library = Library.Create(id, name);

        // Assert
        library.Id.Should().Be(id);
        library.Name.Should().Be(name);
        library.Books.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_RaiseLibraryCreatedDomainEvent()
    {
        // Arrange
        Guid id = LibraryData.GetTestGuid();
        LibraryName name = LibraryData.TestLibraryName;

        // Act
        var library = Library.Create(id, name);

        // Assert
        LibraryCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<LibraryCreatedDomainEvent>(library);
        domainEvent.Library.Should().Be(library);
    }

    [Fact]
    public void AddBook_WithNewBook_Should_Succeed()
    {
        // Arrange
        Library library = LibraryData.CreateTestLibrary();
        Book book = LibraryData.CreateTestBook();

        // Act
        Result result = library.AddBook(book);

        // Assert
        result.IsSuccess.Should().BeTrue();
        library.Books.Should().Contain(book);
    }

    [Fact]
    public void AddBook_WithNewBook_Should_RaiseBookAddedToLibraryDomainEvent()
    {
        // Arrange
        Library library = LibraryData.CreateTestLibrary();
        Book book = LibraryData.CreateTestBook();

        // Act
        library.AddBook(book);

        // Assert
        BookAddedToLibraryDomainEvent domainEvent = AssertDomainEventWasPublished<BookAddedToLibraryDomainEvent>(library);
        domainEvent.Book.Should().Be(book);
    }

    [Fact]
    public void AddBook_WithExistingBook_Should_Fail()
    {
        // Arrange
        Library library = LibraryData.CreateTestLibrary();
        Book book = LibraryData.CreateTestBook();
        library.AddBook(book); // Add the book first

        // Act
        Result result = library.AddBook(book);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LibraryErrors.BookAlreadyExists);
    }
}
