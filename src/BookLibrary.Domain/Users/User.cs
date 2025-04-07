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
    public UserName Name { get; private set; }
    public Email Email { get; private set; }
    public LibraryCard? Card { get; private set; }

    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.ToList();

    public User(Guid id, UserName name, Email email) : base(id)
    {
        Name = name;
        Email = email;  
    }

    public Result AssignCard(LibraryCard card)
    {
        if (card.UserId != Id)
        {
            return Result.Failure(UserErrors.UserAlreadyHasCard);
        }

        Card = card;
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
        RaiseDomainEvent(new BookBorrowedDomainEvent(book));
        return Result.Success();
    }

    public Result ReturnBook(Book book)
    {
        Loan loan = _loans.FirstOrDefault(l => l.BookId == book.Id && !l.IsReturned);

        if (loan == null)
        {
            return Result.Failure(UserErrors.BookNotBorrowed);
        }

        loan.MarkAsReturned();
        book.MarkAsReturned();
        return Result.Success();
    }
}
