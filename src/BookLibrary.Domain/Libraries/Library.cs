using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Libraries.Events;

namespace BookLibrary.Domain.Libraries;

public class Library : Entity
{
    public LibraryName Name { get; private set; }

    private readonly List<Book> _books = new();
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

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

    public void AddBook(Book book)
    {
        _books.Add(book);
        RaiseDomainEvent(new BookAddedToLibraryDomainEvent(book));
    }

    public void RemoveBook(Book book)
    {
        _books.Remove(book);
        RaiseDomainEvent(new BookRemovedFromLibraryDomainEvent(book));
    }
}
