using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.LibraryCards;
using BookLibrary.Domain.Users.Events;

namespace BookLibrary.Domain.Users;

public class User : Entity
{
    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.ToList();
    public Email Email { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public LibraryCard? Card { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.ToList();

    public User(Guid id, Email email, FirstName firstName, LastName lastName, DateTime registeredAt) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        RegisteredAt = registeredAt;
    }

    public static User Create(Guid id, Email email, FirstName firstName, LastName lastName, DateTime registeredAt)
    {
        var user = new User(id, email, firstName, lastName, registeredAt);

        user._roles.Add(Role.Create("User"));

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user));

        return user;
    }

    public Result Update(Email email, FirstName firstName, LastName lastName, DateTime updatedAt)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = updatedAt;

        RaiseDomainEvent(new UserUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result AssignCard(LibraryCard card)
    {
        if (card.UserId != Id)
        {
            return Result.Failure(UserErrors.UserAlreadyHasCard);
        }

        Card = card;
        RaiseDomainEvent(new UserAssignedCardDomainEvent(this, card));

        return Result.Success();
    }

    public bool HasValidCard => Card != null && Card.IsActive;

    public Result BorrowBook(Book book, LoanPeriod period)
    {
        if (!HasValidCard)
        {
            return Result.Failure(UserErrors.UserDoesNotHaveCard);
        }

        if (!book.IsAvailable)
        {
            return Result.Failure(UserErrors.BookNotAvailable);
        }

        var loan = Loan.Create(Guid.NewGuid(), Id, book.Id, period);
        _loans.Add(loan);
        book.MarkAsBorrowed();
        RaiseDomainEvent(new BookBorrowedDomainEvent(this, book, loan));
        return Result.Success();
    }

    public Result ReturnBook(Book book, DateTime returnedAt)
    {
        Loan loan = _loans.FirstOrDefault(l => l.BookId == book.Id && !l.IsReturned);

        if (loan == null)
        {
            return Result.Failure(UserErrors.BookNotBorrowed);
        }

        loan.MarkAsReturned(returnedAt);
        book.MarkAsReturned();

        RaiseDomainEvent(new BookReturnedDomainEvent(this, book, loan));

        return Result.Success();
    }
}
