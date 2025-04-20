using System;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;

namespace BookLibrary.Domain.UnitTests.Users;
internal static class UserData
{
    // Static test data
    public static readonly FirstName FirstName = new("First");
    public static readonly LastName LastName = new("Last");
    public static readonly Email Email = new("hoangdt@gmail.com");
    public static readonly PasswordHash PasswordHash = new("password123");

    // Additional test data for various scenarios
    public static readonly FirstName NewFirstName = new("NewFirst");
    public static readonly LastName NewLastName = new("NewLast");

    // Helper methods for creating test objects
    public static User CreateTestUser()
    {
        return User.Create(
            Guid.NewGuid(),
            Email,
            FirstName,
            LastName,
            PasswordHash,
            DateTime.UtcNow);
    }

    public static User CreateUserWithId(Guid id)
    {
        return User.Create(
            id,
            Email,
            FirstName,
            LastName,
            PasswordHash,
            DateTime.UtcNow);
    }

    public static User CreateUserWithSpecificData(Email email, FirstName firstName, LastName lastName, PasswordHash passwordHash)
    {
        return User.Create(
            Guid.NewGuid(),
            email,
            firstName,
            lastName,
            passwordHash,
            DateTime.UtcNow);
    }

    public static Book CreateAvailableBook()
    {
        var bookTitle = new BookTitle("Test Book");
        var author = new Author("John", "Doe", "USA");
        return Book.Create(Guid.NewGuid(), bookTitle, author, 1, Guid.NewGuid());
    }

    public static Book CreateAvailableBookWithTitle(string title)
    {
        var bookTitle = new BookTitle(title);
        var author = new Author("John", "Doe", "USA");
        return Book.Create(Guid.NewGuid(), bookTitle, author, 1, Guid.NewGuid());
    }

    public static Book CreateUnavailableBook()
    {
        var bookTitle = new BookTitle("Unavailable Book");
        var author = new Author("Jane", "Smith", "UK");
        var book = Book.Create(Guid.NewGuid(), bookTitle, author, 1, Guid.NewGuid());
        
        // Make the book unavailable by marking it as borrowed
        book.MarkAsBorrowed();
        
        return book;
    }

    public static Book CreateDeletedBook()
    {
        var bookTitle = new BookTitle("Deleted Book");
        var author = new Author("Bob", "Wilson", "Canada");
        var book = Book.Create(Guid.NewGuid(), bookTitle, author, 0, Guid.NewGuid());
        
        // Mark as deleted
        book.MarkAsDeleted();
        
        return book;
    }

    public static LoanPeriod CreateLoanPeriod()
    {
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddDays(14); // 2 weeks loan period
        return LoanPeriod.Create(startDate, endDate).Value;
    }

    public static LoanPeriod CreateLoanPeriodWithDuration(int days)
    {
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddDays(days);
        return LoanPeriod.Create(startDate, endDate).Value;
    }

    public static LoanPeriod CreateLoanPeriodWithDates(DateTime startDate, DateTime endDate)
    {
        return LoanPeriod.Create(startDate, endDate).Value;
    }

    // Common test scenarios
    public static DateTime GetTestDateTime()
    {
        return DateTime.UtcNow;
    }

    public static DateTime GetTestDateTimeWithOffset(int minutesOffset)
    {
        return DateTime.UtcNow.AddMinutes(minutesOffset);
    }

    public static Guid GetTestGuid()
    {
        return Guid.NewGuid();
    }
}
