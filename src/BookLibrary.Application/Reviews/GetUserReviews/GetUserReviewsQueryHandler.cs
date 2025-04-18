using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Reviews.GetReview;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.GetUserReviews;

internal sealed class GetUserReviewsQueryHandler : IQueryHandler<GetUserReviewsQuery, List<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetUserReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<List<ReviewResponse>>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
    {
        List<Review> reviews = await _reviewRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var response = reviews.Select(review => new ReviewResponse(
            review.Id,
            review.BookId,
            review.UserId,
            review.Comment.Value,
            review.Rating?.Value,
            review.CreatedAt,
            review.UpdatedAt))
            .ToList();

        return Result.Success(response);
    }
}
