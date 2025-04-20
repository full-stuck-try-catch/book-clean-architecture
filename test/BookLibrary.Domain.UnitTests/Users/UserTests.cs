using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.UnitTests.Infrastructure;
using BookLibrary.Domain.Users;
using BookLibrary.Domain.Users.Events;
using FluentAssertions;

namespace BookLibrary.Domain.UnitTests.Users;
public class UserTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Arrange
        DateTime utcNow = UserData.GetTestDateTime();
        var id = UserData.GetTestGuid();

        // Act
        var user = User.Create(id, UserData.Email, UserData.FirstName, UserData.LastName, UserData.PasswordHash, utcNow);

        // Assert
        user.Id.Should().Be(id);
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
        user.PasswordHash.Should().Be(UserData.PasswordHash);
        user.RegisteredAt.Should().Be(utcNow);
        user.UpdatedAt.Should().BeNull();
        user.Roles.Should().ContainSingle().Which.Should().Be(Role.User);
        user.Loans.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_RaiseUserRegisteredDomainEvent()
    {
        // Arrange
        DateTime utcNow = UserData.GetTestDateTime();
        var id = UserData.GetTestGuid();

        // Act
        var user = User.Create(id, UserData.Email, UserData.FirstName, UserData.LastName, UserData.PasswordHash, utcNow);

        // Assert
        UserRegisteredDomainEvent domainEvent = AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void Update_Should_UpdateUserProperties()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var updatedAt = UserData.GetTestDateTime();

        // Act
        var result = user.Update(UserData.NewFirstName, UserData.NewLastName, updatedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be(UserData.NewFirstName);
        user.LastName.Should().Be(UserData.NewLastName);
        user.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Update_Should_RaiseUserUpdatedDomainEvent()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var updatedAt = UserData.GetTestDateTime();

        // Act
        user.Update(UserData.NewFirstName, UserData.NewLastName, updatedAt);

        // Assert
        UserUpdatedDomainEvent domainEvent = AssertDomainEventWasPublished<UserUpdatedDomainEvent>(user);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void BorrowBook_WithAvailableBook_Should_Succeed()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        var result = user.BorrowBook(book, loanPeriod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Loans.Should().HaveCount(1);
        var loan = user.Loans.First();
        loan.UserId.Should().Be(user.Id);
        loan.BookId.Should().Be(book.Id);
        loan.Period.Should().Be(loanPeriod);
        loan.IsReturned.Should().BeFalse();
    }

    [Fact]
    public void BorrowBook_WithAvailableBook_Should_MarkBookAsBorrowed()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        user.BorrowBook(book, loanPeriod);

        // Assert
        book.Status.Should().Be(BookStatus.Borrowed);
        book.AvailableQuantity.Should().Be(0); // Started with 1, now 0
    }

    [Fact]
    public void BorrowBook_WithAvailableBook_Should_RaiseBookBorrowedDomainEvent()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        user.BorrowBook(book, loanPeriod);

        // Assert
        BookBorrowedDomainEvent domainEvent = AssertDomainEventWasPublished<BookBorrowedDomainEvent>(user);
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.Book.Should().Be(book);
        domainEvent.Loan.Should().NotBeNull();
        domainEvent.Loan.UserId.Should().Be(user.Id);
        domainEvent.Loan.BookId.Should().Be(book.Id);
    }

    [Fact]
    public void BorrowBook_WithUnavailableBook_Should_Fail()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateUnavailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        var result = user.BorrowBook(book, loanPeriod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotAvailable);
        user.Loans.Should().BeEmpty();
    }

    [Fact]
    public void BorrowBook_WithDeletedBook_Should_Fail()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateDeletedBook();
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        var result = user.BorrowBook(book, loanPeriod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotAvailable);
        user.Loans.Should().BeEmpty();
    }

    [Fact]
    public void ReturnBook_WithBorrowedBook_Should_Succeed()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();
        var returnedAt = UserData.GetTestDateTime();

        // First borrow the book
        user.BorrowBook(book, loanPeriod);

        // Act
        var result = user.ReturnBook(book, returnedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var loan = user.Loans.First();
        loan.IsReturned.Should().BeTrue();
        loan.ReturnedAt.Should().Be(returnedAt);
    }

    [Fact]
    public void ReturnBook_WithBorrowedBook_Should_MarkBookAsReturned()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();
        var returnedAt = UserData.GetTestDateTime();

        // First borrow the book
        user.BorrowBook(book, loanPeriod);

        // Act
        user.ReturnBook(book, returnedAt);

        // Assert
        book.Status.Should().Be(BookStatus.Available);
        book.AvailableQuantity.Should().Be(1); // Back to original quantity
    }

    [Fact]
    public void ReturnBook_WithBorrowedBook_Should_RaiseBookReturnedDomainEvent()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();
        var returnedAt = UserData.GetTestDateTime();

        // First borrow the book
        user.BorrowBook(book, loanPeriod);
        user.ClearDomainEvents(); // Clear the borrow event to test only return event

        // Act
        user.ReturnBook(book, returnedAt);

        // Assert
        BookReturnedDomainEvent domainEvent = AssertDomainEventWasPublished<BookReturnedDomainEvent>(user);
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.BookId.Should().Be(book.Id);
        domainEvent.ReturnedAt.Should().Be(returnedAt);
    }

    [Fact]
    public void ReturnBook_WithNotBorrowedBook_Should_Fail()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var returnedAt = UserData.GetTestDateTime();

        // Act
        var result = user.ReturnBook(book, returnedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact]
    public void ReturnBook_WithAlreadyReturnedBook_Should_Fail()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book = UserData.CreateAvailableBook();
        var loanPeriod = UserData.CreateLoanPeriod();
        var returnedAt = UserData.GetTestDateTime();

        // First borrow and return the book
        user.BorrowBook(book, loanPeriod);
        user.ReturnBook(book, returnedAt);

        // Act - Try to return again
        var result = user.ReturnBook(book, UserData.GetTestDateTimeWithOffset(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact] 
    public void ReturnBook_WithDifferentBook_Should_Fail()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var borrowedBook = UserData.CreateAvailableBookWithTitle("Borrowed Book");
        var differentBook = UserData.CreateAvailableBookWithTitle("Different Book");
        var loanPeriod = UserData.CreateLoanPeriod();
        var returnedAt = UserData.GetTestDateTime();

        // Borrow one book
        user.BorrowBook(borrowedBook, loanPeriod);

        // Act - Try to return a different book
        var result = user.ReturnBook(differentBook, returnedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact]
    public void BorrowBook_MultipleDifferentBooks_Should_Succeed()
    {
        // Arrange
        var user = UserData.CreateTestUser();
        var book1 = UserData.CreateAvailableBookWithTitle("Book 1");
        var book2 = UserData.CreateAvailableBookWithTitle("Book 2");
        var loanPeriod = UserData.CreateLoanPeriod();

        // Act
        var result1 = user.BorrowBook(book1, loanPeriod);
        var result2 = user.BorrowBook(book2, loanPeriod);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        user.Loans.Should().HaveCount(2);
        user.Loans.Should().OnlyContain(l => !l.IsReturned);
    }

    [Fact]
    public void Constructor_WithParameters_Should_SetProperties()
    {
        // Arrange
        var id = UserData.GetTestGuid();
        var registeredAt = UserData.GetTestDateTime();

        // Act
        var user = new User(id, UserData.Email, UserData.FirstName, UserData.LastName, UserData.PasswordHash, registeredAt);

        // Assert
        user.Id.Should().Be(id);
        user.Email.Should().Be(UserData.Email);
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.PasswordHash.Should().Be(UserData.PasswordHash);
        user.RegisteredAt.Should().Be(registeredAt);
        user.UpdatedAt.Should().BeNull();
        user.Roles.Should().BeEmpty(); // Constructor doesn't add roles
        user.Loans.Should().BeEmpty();
    }
}
