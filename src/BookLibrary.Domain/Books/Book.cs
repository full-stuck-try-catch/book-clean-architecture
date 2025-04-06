using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books.Events;

namespace BookLibrary.Domain.Books;

public class Book : Entity
{
    public BookTitle Title { get; private set; }
    public AuthorName Author { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public Book(Guid id, BookTitle title, AuthorName author) : base(id)
    {
        Title = title;
        Author = author;
    }

    public static Book Create(Guid id, BookTitle title, AuthorName author)
{
       var book = new Book(id, title, author);

       book.RaiseDomainEvent(new BookCreatedDomainEvent(book));

       return book;
}
    public void MarkAsBorrowed()
    {
        IsAvailable = false;
        RaiseDomainEvent(new BookBorrowedDomainEvent(this));
    }

    public void MarkAsReturned()
    {
        IsAvailable = true;
        RaiseDomainEvent(new BookReturnedDomainEvent(this));
    }
}
