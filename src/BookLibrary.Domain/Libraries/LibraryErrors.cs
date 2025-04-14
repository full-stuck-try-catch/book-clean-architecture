using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries;

public static class LibraryErrors
{
    public static Error BookAlreadyExists => new("Book.AlreadyExists", "Book already exists in the library.");
    public static Error UserAlreadyExists => new("User.AlreadyExists", "User already exists in the library.");
    public static Error CardNumberAlreadyExists => new("Card.NumberAlreadyExists", "Card number already exists.");
    
    // Additional errors for library operations
    public static Error NotFound => new("Library.NotFound", "The library with the specified identifier was not found.");
    public static Error NameAlreadyExists => new("Library.NameAlreadyExists", "A library with the specified name already exists.");
    public static Error InvalidName => new("Library.InvalidName", "The library name is invalid.");
    public static Error BookNotFound => new("Library.BookNotFound", "The book was not found in the library.");
}
