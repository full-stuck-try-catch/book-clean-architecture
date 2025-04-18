using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Reviews.GetReview;

namespace BookLibrary.Application.Reviews.GetUserReviews;

public sealed record GetUserReviewsQuery(Guid UserId) : IQuery<List<ReviewResponse>>;
