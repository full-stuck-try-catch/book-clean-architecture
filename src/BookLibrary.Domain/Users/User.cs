using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users.Events;

namespace BookLibrary.Domain.Users;

public class User : Entity
{
    private readonly List<Role> _roles = new();
     private readonly List<Loan> _loans = new();

    public IReadOnlyCollection<Role> Roles => _roles.ToList();
    public Email Email { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public IReadOnlyCollection<Loan> Loans => _loans.ToList();
    public DateTime RegisteredAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private User()
    {
    }

    public User(Guid id, Email email, FirstName firstName, LastName lastName, PasswordHash passwordHash, DateTime registeredAt) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        RegisteredAt = registeredAt;
    }

    public static User Create(Guid id, Email email, FirstName firstName, LastName lastName, PasswordHash passwordHash, DateTime registeredAt)
    {
        var user = new User(id, email, firstName, lastName, passwordHash, registeredAt);

        user._roles.Add(Role.User);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user));

        return user;
    }

    public Result Update(FirstName firstName, LastName lastName, DateTime updatedAt)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = updatedAt;

        RaiseDomainEvent(new UserUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result BorrowBook(Book book, LoanPeriod period)
    {

        if (book.Status != BookStatus.Available)
        {
        return Result.Failure(UserErrors.BookNotAvailable);
        }

        book.MarkAsBorrowed();

        Result<Loan> loanResult = Loan.Create(Guid.NewGuid(), Id, book.Id, period);

        if (loanResult.IsFailure)
        {
            return loanResult;
        }

        _loans.Add(loanResult.Value);

        RaiseDomainEvent(new BookBorrowedDomainEvent(Id, book, loanResult.Value));

        return Result.Success();
    }

    public Result ReturnBook(Book book, DateTime returnedAt)
    {
        Loan? loan = _loans.Find(l => l.BookId == book.Id && !l.IsReturned);

        if (loan == null)
        {
            return Result.Failure(UserErrors.BookNotBorrowed);
        }

        loan.MarkAsReturned(returnedAt);

        book.MarkAsReturned();

        RaiseDomainEvent(new BookReturnedDomainEvent(Id, loan.BookId, returnedAt));

        return Result.Success();
    }
}
