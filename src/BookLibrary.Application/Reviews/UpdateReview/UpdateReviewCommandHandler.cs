using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.UpdateReview;

internal sealed class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
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

        // Create comment
        Result<Comment> commentResult =  new Comment(request.Comment);
        if (commentResult.IsFailure)
        {
            return Result.Failure(commentResult.Error);
        }

        // Create rating if provided
        Rating? rating = null;
        if (request.Rating.HasValue)
        {
            Result<Rating> ratingResult = Rating.Create(request.Rating.Value);
            if (ratingResult.IsFailure)
            {
                return Result.Failure(ratingResult.Error);
            }
            rating = ratingResult.Value;
        }

        // Update review
        Result updateResult = review.Update(
            commentResult.Value,
            rating,
            _dateTimeProvider.UtcNow);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        _reviewRepository.Update(review);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
