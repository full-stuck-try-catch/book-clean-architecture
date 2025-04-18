using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Reviews.CreateReview;

public sealed record CreateReviewCommand(
    Guid BookId,
    string Comment,
    int? Rating) : ICommand<Guid>;
