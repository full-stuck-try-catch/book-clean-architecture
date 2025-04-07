using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Libraries.Events;
using BookLibrary.Domain.Users;
using BookLibrary.Domain.LibraryCards;

namespace BookLibrary.Domain.Libraries;

public class Library : AggregateRoot
{
    public LibraryName Name { get; private set; }
    private readonly List<Book> _books = new();
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();
    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    private readonly List<LibraryCard> _cards = new();
    public IReadOnlyCollection<LibraryCard> Cards => _cards.AsReadOnly();

    private Library(Guid id, LibraryName name) : base(id)
    {
        Name = name;
    }

    public static Library Create(Guid id, LibraryName name)
    {
        var library = new Library(id, name);
        library.RaiseDomainEvent(new LibraryCreatedDomainEvent(library));
        return library;
    }

    public Result AddBook(Book book)
    {
        if (_books.Any(b => b.Title == book.Title && b.Author == book.Author))
        {
            return Result.Failure(LibraryErrors.BookAlreadyExists);
        }

        _books.Add(book);
        RaiseDomainEvent(new BookAddedToLibraryDomainEvent(book));
        return Result.Success();
    }

    public void RemoveBook(Book book)
    {
        _books.Remove(book);
        RaiseDomainEvent(new BookRemovedFromLibraryDomainEvent(book));
    }

    public Result RegisterUser(User user)
    {
            if (_users.Any(u => u.Email == user.Email))
            {
                return Result.Failure(LibraryErrors.UserAlreadyExists);
            }

        _users.Add(user);
        return Result.Success();
    }

     public LibraryCard IssueLibraryCard(User user, LibraryCardNumber cardNumber)
    {
        if (user.Card != null && user.Card.IsActive)
        {
            throw new InvalidOperationException("User already has an active library card.");
        }

        if (_cards.Any(c => c.CardNumber == cardNumber))
        {
            throw new InvalidOperationException("Card number already exists.");
        }

        var card = LibraryCard.Create(Guid.NewGuid(), cardNumber, user.Id, DateTime.UtcNow);
        _cards.Add(card);
        user.AssignCard(card);
        return card;
    }

}
