using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Loans.Events;
using BookLibrary.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace BookLibrary.Domain.UnitTests.Loans;

public class LoanTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Arrange
        Guid id = LoanData.GetTestGuid();
        Guid userId = LoanData.TestUserId;
        Guid bookId = LoanData.TestBookId;
        LoanPeriod period = LoanData.CreateLoanPeriod();

        // Act
        Result<Loan> result = Loan.Create(id, userId, bookId, period);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Loan loan = result.Value;
        loan.Id.Should().Be(id);
        loan.UserId.Should().Be(userId);
        loan.BookId.Should().Be(bookId);
        loan.Period.Should().Be(period);
        loan.IsReturned.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_RaiseLoanCreatedDomainEvent()
    {
        // Arrange
        Guid id = LoanData.GetTestGuid();
        Guid userId = LoanData.TestUserId;
        Guid bookId = LoanData.TestBookId;
        LoanPeriod period = LoanData.CreateLoanPeriod();

        // Act
        var loan = Loan.Create(id, userId, bookId, period).Value;

        // Assert
        LoanCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<LoanCreatedDomainEvent>(loan);
        domainEvent.Loan.Should().Be(loan);
    }

    [Fact]
    public void MarkAsReturned_WithActiveLoan_Should_Succeed()
    {
        // Arrange
        Loan loan = LoanData.CreateTestLoan();
        DateTime returnedAt = DateTime.UtcNow;

        // Act
        Result result = loan.MarkAsReturned(returnedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        loan.IsReturned.Should().BeTrue();
        loan.ReturnedAt.Should().Be(returnedAt);
    }

    [Fact]
    public void MarkAsReturned_WithActiveLoan_Should_RaiseLoanReturnedDomainEvent()
    {
        // Arrange
        Loan loan = LoanData.CreateTestLoan();
        DateTime returnedAt = DateTime.UtcNow;

        // Act
        loan.MarkAsReturned(returnedAt);

        // Assert
        LoanReturnedDomainEvent domainEvent = AssertDomainEventWasPublished<LoanReturnedDomainEvent>(loan);
        domainEvent.Loan.Should().Be(loan);
    }

    [Fact]
    public void MarkAsReturned_WithAlreadyReturnedLoan_Should_Fail()
    {
        // Arrange
        Loan loan = LoanData.CreateReturnedLoan();
        DateTime returnedAt = DateTime.UtcNow;

        // Act
        Result result = loan.MarkAsReturned(returnedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.LoanAlreadyReturned);
    }

    [Fact]
    public void Extend_WithValidExtension_Should_Succeed()
    {
        // Arrange
        Loan loan = LoanData.CreateTestLoan();
        LoanPeriod extension = LoanPeriod.Create(loan.Period.EndDate, loan.Period.EndDate.AddDays(7)).Value;

        // Act
        Result result = loan.Extend(extension);

        // Assert
        result.IsSuccess.Should().BeTrue();
        loan.Period.Should().Be(extension);
    }

    [Fact]
    public void Extend_WithValidExtension_Should_RaiseLoanExtendedDomainEvent()
    {
        // Arrange
        Loan loan = LoanData.CreateTestLoan();
        LoanPeriod extension = LoanPeriod.Create(loan.Period.EndDate, loan.Period.EndDate.AddDays(7)).Value;

        // Act
        loan.Extend(extension);

        // Assert
        LoanExtendedDomainEvent domainEvent = AssertDomainEventWasPublished<LoanExtendedDomainEvent>(loan);
        domainEvent.Loan.Should().Be(loan);
        domainEvent.Extension.Should().Be(extension);
    }

    [Fact]
    public void Extend_WithInvalidExtension_Should_Fail()
    {
        // Arrange
        Loan loan = LoanData.CreateTestLoan();
        LoanPeriod invalidExtension = LoanPeriod.Create(loan.Period.StartDate, loan.Period.EndDate).Value;

        // Act
        Result result = loan.Extend(invalidExtension);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.InvalidExtension);
    }

    [Fact]
    public void Extend_WithReturnedLoan_Should_Fail()
    {
        // Arrange
        Loan loan = LoanData.CreateReturnedLoan();
        LoanPeriod extension = LoanPeriod.Create(loan.Period.EndDate, loan.Period.EndDate.AddDays(7)).Value;

        // Act
        Result result = loan.Extend(extension);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoanErrors.LoanAlreadyReturned);
    }
}
