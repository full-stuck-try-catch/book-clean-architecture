using BookLibrary.Domain.Books;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Domain.UnitTests.Reviews;

internal static class ReviewData
{
    public static Guid GetTestGuid() => Guid.NewGuid();

    public static Guid TestUserId => Guid.NewGuid();
    public static Comment TestComment => new("This is a test comment.");
    public static Rating TestRating => Rating.Create(4).Value;
    public static DateTime TestCreationDate => DateTime.UtcNow;

    public static Book CreateAvailableBook()
    {
        return Book.Create(
            Guid.NewGuid(),
            new BookTitle("Available Book"),
            new Author("Test", "Author", "USA"),
            1,
            Guid.NewGuid());
    }

    public static Book CreateDeletedBook()
    {
        var book = Book.Create(
            Guid.NewGuid(),
            new BookTitle("Deleted Book"),
            new Author("Test", "Author", "USA"),
            0,
            Guid.NewGuid());
        book.MarkAsDeleted();
        book.ClearDomainEvents();
        return book;
    }

    public static Review CreateTestReview()
    {
        return Review.Create(
            GetTestGuid(),
            CreateAvailableBook(),
            TestUserId,
            TestComment,
            TestRating,
            TestCreationDate).Value;
    }
}
