using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users.Events;

namespace BookLibrary.Domain.Users;

public sealed class User : Entity
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

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user));

        return user;
    }

    public void AddRole(Role role)
    {
        if (_roles.Contains(role))
        {
            return;
        }
        _roles.Add(role);
    }

    public Result Update(FirstName firstName, LastName lastName, DateTime updatedAt)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = updatedAt;

        RaiseDomainEvent(new UserUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result BorrowBook(Book book, Loan loan)
    {
        if (book.Status != BookStatus.Available)
        {
            return Result.Failure(UserErrors.BookNotAvailable);
        }

        if (_loans.Any(l => l.BookId == loan.BookId && !l.IsReturned))
        {
            return Result.Failure(UserErrors.LoanAlreadyExists);
        }

        _loans.Add(loan);
        book.MarkAsBorrowed();

        RaiseDomainEvent(new BookBorrowedDomainEvent(Id, loan));

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
