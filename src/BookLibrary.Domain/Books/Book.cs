using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books.Events;

namespace BookLibrary.Domain.Books;

public class Book : Entity
{
    public BookTitle Title { get; private set; }
    public AuthorName Author { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public int Quantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    public Book(Guid id, BookTitle title, AuthorName author, int quantity) : base(id)
    {
        Title = title;
        Author = author;
        Quantity = quantity;
        AvailableQuantity = quantity;
    }

    public static Book Create(Guid id, BookTitle title, AuthorName author, int quantity)
    {
        var book = new Book(id, title, author, quantity);

        book.RaiseDomainEvent(new BookCreatedDomainEvent(book));

        return book;
    }
    public Result MarkAsBorrowed()
    {
        if(AvailableQuantity <= 0){
            return Result.Failure(BookErrors.BookNotAvailable);
        }

        AvailableQuantity--;
        RaiseDomainEvent(new BookBorrowedDomainEvent(this));
        return Result.Success();
    }

    public Result MarkAsReturned()
    {
        if(AvailableQuantity >= Quantity){
            return Result.Failure(BookErrors.BookAllAlreadyReturned);
        }

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

    public Result RemoveStock(int count)
    {
        if (count <= 0 || count > AvailableQuantity)
        {
            return Result.Failure(BookErrors.InvalidStockCount);
        }

        Quantity -= count;
        AvailableQuantity -= count;
        return Result.Success();
    }
}
