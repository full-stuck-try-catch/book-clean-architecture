using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries;

public static class LibraryErrors
{
    public static Error BookAlreadyExists => new("Book.AlreadyExists", "Book already exists in the library.");
    public static Error UserAlreadyExists => new("User.AlreadyExists", "User already exists in the library.");
}
