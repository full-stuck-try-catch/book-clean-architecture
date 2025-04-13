using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Reviews.Events;

namespace BookLibrary.Domain.Reviews;
public sealed class Review : Entity
{
    public Guid BookId { get; private set; }

    public Guid UserId { get; private set; }

    public Comment Comment { get; private set; }

    public Rating? Rating { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public Review(Guid id, Guid bookId, Guid userId, Comment comment, Rating? rating, DateTime createdAt) : base(id)
    {
        BookId = bookId;
        UserId = userId;
        Comment = comment;
        Rating = rating;
        CreatedAt = createdAt;
    }

    public static Result<Review> Create(Guid id, Book book, Guid userId, Comment comment, Rating? rating, DateTime createdAt)
    {
        if (book.Status == BookStatus.Deleted)
        {
            return Result.Failure<Review>(ReviewErrors.BookNotAvailable);
        }

        var review = new Review(id, book.Id, userId, comment, rating, createdAt);

        review.RaiseDomainEvent(new ReviewCreatedDomainEvent(review));
        return review;
    }

    public Result Update(Comment comment, Rating? rating, DateTime updatedAt)
    {
        Comment = comment;
        Rating = rating;
        UpdatedAt = updatedAt;
        return Result.Success();
    }
}
