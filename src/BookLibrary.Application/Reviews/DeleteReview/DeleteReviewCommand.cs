using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Reviews.DeleteReview;

public sealed record DeleteReviewCommand(Guid ReviewId) : ICommand;
