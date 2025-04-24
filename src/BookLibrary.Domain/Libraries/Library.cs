using BookLibrary.Domain.Books;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Libraries.Events;

namespace BookLibrary.Domain.Libraries;

public sealed class Library : AggregateRoot
{
    public LibraryName Name { get; private set; }
    private readonly List<Book> _books = new();
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    private Library() { }

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
}
