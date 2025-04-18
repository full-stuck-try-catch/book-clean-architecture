using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Reviews.UpdateReview;

public sealed record UpdateReviewCommand(
    Guid ReviewId,
    string Comment,
    int? Rating) : ICommand;
