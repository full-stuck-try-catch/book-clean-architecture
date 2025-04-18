using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Application.Reviews.GetReview;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.GetBookReviews;

internal sealed class GetBookReviewsQueryHandler : IQueryHandler<GetBookReviewsQuery, List<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetBookReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<List<ReviewResponse>>> Handle(GetBookReviewsQuery request, CancellationToken cancellationToken)
    {
        List<Review> reviews = await _reviewRepository.GetByBookIdAsync(request.BookId, cancellationToken);

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
