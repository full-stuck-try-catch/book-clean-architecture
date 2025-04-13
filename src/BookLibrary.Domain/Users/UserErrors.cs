using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users;

public static class UserErrors
{
    public static Error UserAlreadyHasActiveCard => new("User.AlreadyHasActiveCard", "User already has an active library card.");
    public static Error UserAlreadyHasCard => new("User.AlreadyHasCard", "User already has a library card.");
    public static Error UserDoesNotHaveCard => new("User.DoesNotHaveCard", "User does not have a library card.");
    public static Error BookNotAvailable => new("Book.NotAvailable", "Book is not available.");
    public static Error BookNotBorrowed => new("Book.NotBorrowed", "Book is not borrowed.");
    public static Error CardIsNotActive => new("Card.NotActive", "Card is not active.");
    
    // Authentication errors
    public static Error NotFound => new("User.NotFound", "The user with the specified identifier was not found.");
    public static Error InvalidCredentials => new("User.InvalidCredentials", "The provided credentials are invalid.");
    public static Error EmailAlreadyInUse => new("User.EmailAlreadyInUse", "The specified email is already in use.");
    public static Error InvalidEmailFormat => new("User.InvalidEmailFormat", "The email format is invalid.");
    public static Error PasswordTooShort => new("User.PasswordTooShort", "The password must be at least 6 characters long.");
}

