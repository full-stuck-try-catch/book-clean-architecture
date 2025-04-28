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
        Guid id = UserData.GetTestGuid();

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
        Guid id = UserData.GetTestGuid();

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
        User user = UserData.CreateTestUser();
        DateTime updatedAt = UserData.GetTestDateTime();

        // Act
        Result result = user.Update(UserData.NewFirstName, UserData.NewLastName, updatedAt);

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
        User user = UserData.CreateTestUser();
        DateTime updatedAt = UserData.GetTestDateTime();

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
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);

        // Act
        Result result = user.BorrowBook(book, loan.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Loans.Should().HaveCount(1);
    }

    [Fact]
    public void BorrowBook_WithAvailableBook_Should_MarkBookAsBorrowed()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        // Act
        user.BorrowBook(book, loan.Value);
        // Assert
        book.Status.Should().Be(BookStatus.Borrowed);
        book.AvailableQuantity.Should().Be(0); // Started with 1, now 0
    }

    [Fact]
    public void BorrowBook_WithAvailableBook_Should_RaiseBookBorrowedDomainEvent()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        // Act
        user.BorrowBook(book, loan.Value);
        // Assert
        BookBorrowedDomainEvent domainEvent = AssertDomainEventWasPublished<BookBorrowedDomainEvent>(user);
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.Loan.Should().NotBeNull();
        domainEvent.Loan.UserId.Should().Be(user.Id);
        domainEvent.Loan.BookId.Should().Be(book.Id);
    }

    [Fact]
    public void BorrowBook_WithUnavailableBook_Should_Fail()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateUnavailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        // Act
        Result result = user.BorrowBook(book, loan.Value);
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotAvailable);
        user.Loans.Should().BeEmpty();
    }

    [Fact]
    public void BorrowBook_WithDeletedBook_Should_Fail()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateDeletedBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        // Act
        Result result = user.BorrowBook(book, loan.Value);
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotAvailable);
        user.Loans.Should().BeEmpty();
    }

    [Fact]
    public void ReturnBook_WithBorrowedBook_Should_Succeed()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        DateTime returnedAt = UserData.GetTestDateTime();
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        // First borrow the book
        user.BorrowBook(book, loan.Value);
        // Act
        Result result = user.ReturnBook(book, returnedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Loans.Should().HaveCount(1);
        user.Loans.First().IsReturned.Should().BeTrue();
        user.Loans.First().ReturnedAt.Should().Be(returnedAt);
    }

    [Fact]
    public void ReturnBook_WithBorrowedBook_Should_MarkBookAsReturned()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        DateTime returnedAt = UserData.GetTestDateTime();

        // First borrow the book
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        user.BorrowBook(book, loan.Value);
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
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        DateTime returnedAt = UserData.GetTestDateTime();

        // First borrow the book
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        user.BorrowBook(book, loan.Value);
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
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        DateTime returnedAt = UserData.GetTestDateTime();

        // Act
        Result result = user.ReturnBook(book, returnedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact]
    public void ReturnBook_WithAlreadyReturnedBook_Should_Fail()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book = UserData.CreateAvailableBook();
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        DateTime returnedAt = UserData.GetTestDateTime();

        // First borrow and return the book
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, book.Id, loanPeriod);
        user.BorrowBook(book, loan.Value);
        user.ReturnBook(book, returnedAt);

        // Act - Try to return again
        Result result = user.ReturnBook(book, UserData.GetTestDateTimeWithOffset(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact] 
    public void ReturnBook_WithDifferentBook_Should_Fail()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book borrowedBook = UserData.CreateAvailableBookWithTitle("Borrowed Book");
        Book differentBook = UserData.CreateAvailableBookWithTitle("Different Book");
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();
        DateTime returnedAt = UserData.GetTestDateTime();

        // Borrow one book
        Result<Loan> loan = Loan.Create(Guid.NewGuid(), user.Id, borrowedBook.Id, loanPeriod);
        user.BorrowBook(borrowedBook, loan.Value);
        // Act - Try to return a different book
        Result result = user.ReturnBook(differentBook, returnedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BookNotBorrowed);
    }

    [Fact]
    public void BorrowBook_MultipleDifferentBooks_Should_Succeed()
    {
        // Arrange
        User user = UserData.CreateTestUser();
        Book book1 = UserData.CreateAvailableBookWithTitle("Book 1");
        Book book2 = UserData.CreateAvailableBookWithTitle("Book 2");
        LoanPeriod loanPeriod = UserData.CreateLoanPeriod();

        // Act
        Result<Loan> loan1 = Loan.Create(Guid.NewGuid(), user.Id, book1.Id, loanPeriod);
        Result<Loan> loan2 = Loan.Create(Guid.NewGuid(), user.Id, book2.Id, loanPeriod);
        Result result1 = user.BorrowBook(book1, loan1.Value);
        Result result2 = user.BorrowBook(book2, loan2.Value);
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
        Guid id = UserData.GetTestGuid();
        DateTime registeredAt = UserData.GetTestDateTime();

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
