using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users;

public static class UserErrors
{
    public static Error UserAlreadyHasActiveCard => new("User.AlreadyHasActiveCard", "User already has an active library card.");
    public static Error UserAlreadyHasCard => new("User.AlreadyHasCard", "User already has a library card.");
    public static Error UserDoesNotHaveCard => new("User.DoesNotHaveCard", "User does not have a library card.");
    public static Error BookNotAvailable => new("Book.NotAvailable", "Book is not available.");
    public static Error BookNotBorrowed => new("Book.NotBorrowed", "Book is not borrowed.");
}

