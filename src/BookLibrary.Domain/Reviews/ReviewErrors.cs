using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Reviews;

public static class ReviewErrors
{
    public static Error BookNotAvailable => new("Review.BookNotAvailable", "Cannot create review for deleted book.");
    
    public static Error NotFound => new("Review.NotFound", "The review was not found.");
    
    public static Error AlreadyExists => new("Review.AlreadyExists", "A review for this book already exists for the current user.");
    
    public static Error Unauthorized => new("Review.Unauthorized", "You are not authorized to modify this review.");
}
