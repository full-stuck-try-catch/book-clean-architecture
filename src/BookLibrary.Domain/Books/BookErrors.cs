using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public static class BookErrors
{
    public static Error BookNotAvailable => new("Book.NotAvailable", "Book is not available.");
    public static Error BookAllAlreadyReturned => new("Book.BookAllAlreadyReturned", "All copies of the book are already returned.");
    public static Error InvalidStockCount => new("Book.InvalidStockCount", "Invalid stock count.");
}