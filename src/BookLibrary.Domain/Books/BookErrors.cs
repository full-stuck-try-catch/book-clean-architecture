using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public static class BookErrors
{
    public static Error BookNotAvailable => new("Book.NotAvailable", "Book is not available.");
    public static Error BookAlreadyDeleted => new("Book.AlreadyDeleted", "Book is already deleted.");
    public static Error BookStillAvailable => new("Book.StillAvailable", "Book is still available and cannot be deleted.");
    public static Error BookAlreadyExists => new("Book.AlreadyExists", "Book with the same title and author already exists.");
    public static Error InvalidStockCount => new("Book.InvalidStockCount", "Invalid stock count.");
    public static Error NotFound => new("Book.NotFound", "The book with the specified identifier was not found.");
    public static Error InvalidTitle => new("Book.InvalidTitle", "The book title is invalid.");
    public static Error InvalidAuthor => new("Book.InvalidAuthor", "The book author information is invalid.");
    public static Error InvalidQuantity => new("Book.InvalidQuantity", "The quantity must be greater than zero.");
    public static Error BookNotBorrowed => new("Book.NotBorrowed", "The book is not currently borrowed.");
}
