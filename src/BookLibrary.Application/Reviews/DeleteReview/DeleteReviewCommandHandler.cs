using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.DeleteReview;

internal sealed class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Get the review
        Review? review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure(ReviewErrors.NotFound);
        }

        // Check if the current user owns this review
        if (review.UserId != userId)
        {
            return Result.Failure(ReviewErrors.Unauthorized);
        }

        _reviewRepository.Remove(review);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
