using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.GetReview;

public sealed class GetReviewQueryHandler : IQueryHandler<GetReviewQuery, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserContext _userContext;

    public GetReviewQueryHandler(
        IReviewRepository reviewRepository,
        IUserContext userContext)
    {
        _reviewRepository = reviewRepository;
        _userContext = userContext;
    }

    public async Task<Result<ReviewResponse>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        Review? review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);

        if (review is null)
        {
            return Result.Failure<ReviewResponse>(ReviewErrors.NotFound);
        }

        // Only allow users to see their own reviews or make this public if needed
        if (review.UserId != userId)
        {
            return Result.Failure<ReviewResponse>(ReviewErrors.Unauthorized);
        }

        var response = new ReviewResponse(
            review.Id,
            review.BookId,
            review.UserId,
            review.Comment.Value,
            review.Rating?.Value,
            review.CreatedAt,
            review.UpdatedAt);

        return Result.Success(response);
    }
}
