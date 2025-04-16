using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books.Events;

namespace BookLibrary.Domain.Books;

public sealed class Book : Entity
{
    public BookTitle Title { get; private set; }
    public Author Author { get; private set; }
    public Guid LibraryId { get; private set; } 
    public int Quantity { get; private set; }
    public int AvailableQuantity { get; private set; }
    public BookStatus Status { get; private set; }

    private Book() { }

    public Book(Guid id, BookTitle title, Author author, int quantity, Guid libraryId) : base(id)
    {
        Title = title;
        Author = author;
        Quantity = quantity;
        AvailableQuantity = quantity;
        Status = BookStatus.Available;
        LibraryId = libraryId;
    }

    public static Book Create(Guid id, BookTitle title, Author author, int quantity, Guid libraryId)
    {
        var book = new Book(id, title, author, quantity, libraryId);

        book.RaiseDomainEvent(new BookCreatedDomainEvent(id , title , author , quantity , libraryId));

        return book;
    }

    public Result MarkAsBorrowed()
    {
        if(AvailableQuantity <= 0){
            return Result.Failure(BookErrors.BookNotAvailable);
        }

        Status = BookStatus.Borrowed;
        AvailableQuantity--;
        RaiseDomainEvent(new BookBorrowedDomainEvent(this));
        return Result.Success();
    }

    public Result MarkAsReturned()
    {
        Status = BookStatus.Available;
        AvailableQuantity++;
        RaiseDomainEvent(new BookReturnedDomainEvent(this));
        return Result.Success();
    }

    public Result AddStock(int count)
    {
        if (count <= 0)
        {
            return Result.Failure(BookErrors.InvalidStockCount);
        }

        Quantity += count;
        AvailableQuantity += count;
        return Result.Success();
    }

    public Result MarkAsDeleted()
    {
        if (Status == BookStatus.Deleted)
        {
            return Result.Failure(BookErrors.BookAlreadyDeleted);
        }

        if (AvailableQuantity > 0)
        {
            return Result.Failure(BookErrors.BookStillAvailable);
        }

        Status = BookStatus.Deleted;
        return Result.Success();
    }
}
