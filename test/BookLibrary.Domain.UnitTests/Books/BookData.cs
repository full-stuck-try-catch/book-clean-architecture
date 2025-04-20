using System;
using BookLibrary.Domain.Books;

namespace BookLibrary.Domain.UnitTests.Books;

internal static class BookData
{
    public static readonly BookTitle TestTitle = new("Test Book");
    public static readonly Author TestAuthor = new("John", "Doe", "USA");
    public const int TestQuantity = 5;
    public static readonly Guid TestLibraryId = Guid.NewGuid();

    public static Book CreateTestBook()
    {
        return Book.Create(
            Guid.NewGuid(),
            TestTitle,
            TestAuthor,
            TestQuantity,
            TestLibraryId);
    }

    public static Book CreateBookWithQuantity(int quantity)
    {
        return Book.Create(
            Guid.NewGuid(),
            TestTitle,
            TestAuthor,
            quantity,
            TestLibraryId);
    }

    public static Book CreateBorrowedBook()
    {
        var book = Book.Create(
            Guid.NewGuid(),
            new BookTitle("Borrowed Book"),
            TestAuthor,
            1,
            TestLibraryId);
        book.MarkAsBorrowed();
        book.ClearDomainEvents();
        return book;
    }
    
    public static Book CreateBookWithNoAvailableStock()
    {
        var book = Book.Create(
            Guid.NewGuid(),
            new BookTitle("No Stock Book"),
            TestAuthor,
            0,
            TestLibraryId);
        book.ClearDomainEvents();
        return book;
    }

    public static Book CreateDeletedBook()
    {
        Book book = CreateBookWithNoAvailableStock();
        book.MarkAsDeleted();
        book.ClearDomainEvents();
        return book;
    }

    public static Guid GetTestGuid()
    {
        return Guid.NewGuid();
    }
}
