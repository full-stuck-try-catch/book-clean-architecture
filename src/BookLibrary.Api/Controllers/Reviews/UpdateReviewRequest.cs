namespace BookLibrary.Api.Controllers.Reviews;

public sealed record UpdateReviewRequest(
    string Comment,
    int? Rating);
