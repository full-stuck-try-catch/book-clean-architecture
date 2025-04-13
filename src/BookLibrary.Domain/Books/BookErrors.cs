using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public static class BookErrors
{
    public static Error BookNotAvailable => new("Book.NotAvailable", "Book is not available.");
    public static Error BookAlreadyDeleted => new("Book.AlreadyDeleted", "Book is already deleted.");
    public static Error BookStillAvailable => new("Book.StillAvailable", "Book is still available and cannot be deleted.");
    public static Error BookAlreadyExists => new("Book.AlreadyExists", "Book with the same title and author already exists.");
    public static Error InvalidStockCount => new("Book.InvalidStockCount", "Invalid stock count.");
}
