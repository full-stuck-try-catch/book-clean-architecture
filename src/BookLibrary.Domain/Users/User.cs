using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Shared;

namespace BookLibrary.Domain.Users;

public class User : Entity
{
    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.ToList();
    public UserName Name { get; private set; }
    public Email Email { get; private set; }

    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.ToList();

    public User(Guid id, UserName name, Email email) : base(id)
    {
        Name = name;
        Email = email;  
    }

    public void BorrowBook(Book book, LoanPeriod period)
    {
        if (!book.IsAvailable)
        {
            throw new InvalidOperationException("Book is not available.");
        }

        var loan = Loan.Create(Guid.NewGuid(), Id, book.Id, period);
        _loans.Add(loan);
        book.MarkAsBorrowed();
    }

    public void ReturnBook(Book book)
    {
        Loan loan = _loans.FirstOrDefault(l => l.BookId == book.Id && !l.IsReturned) ?? throw new InvalidOperationException("No active loan found for this book.");
        loan.MarkAsReturned();
        book.MarkAsReturned();
    }
}
