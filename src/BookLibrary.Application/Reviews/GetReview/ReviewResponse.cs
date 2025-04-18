namespace BookLibrary.Application.Reviews.GetReview;

public sealed record ReviewResponse(
    Guid Id,
    Guid BookId,
    Guid UserId,
    string Comment,
    int? Rating,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
