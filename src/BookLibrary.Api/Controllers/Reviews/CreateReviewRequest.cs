namespace BookLibrary.Api.Controllers.Reviews;

public sealed record CreateReviewRequest(
    Guid BookId,
    string Comment,
    int? Rating);
