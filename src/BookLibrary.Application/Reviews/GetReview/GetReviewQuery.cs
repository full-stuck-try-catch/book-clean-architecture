using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Reviews.GetReview;

public sealed record GetReviewQuery(Guid ReviewId) : IQuery<ReviewResponse>;
