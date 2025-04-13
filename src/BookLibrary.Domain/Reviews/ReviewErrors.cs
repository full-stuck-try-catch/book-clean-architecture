using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Reviews;

public static class ReviewErrors
{
    public static Error BookNotAvailable => new("Review.BookNotAvailable", "Cannot create review for deleted book.");
} 