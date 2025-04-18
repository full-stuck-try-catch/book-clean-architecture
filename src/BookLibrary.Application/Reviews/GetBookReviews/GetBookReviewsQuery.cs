using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Reviews.GetReview;

namespace BookLibrary.Application.Reviews.GetBookReviews;

public sealed record GetBookReviewsQuery(Guid BookId) : IQuery<List<ReviewResponse>>;
