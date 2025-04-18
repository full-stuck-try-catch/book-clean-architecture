using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Reviews;

namespace BookLibrary.Application.Reviews.CreateReview;

internal sealed class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, Guid>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBookRepository bookRepository,
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _bookRepository = bookRepository;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Check if book exists
        Book? book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure<Guid>(BookErrors.NotFound);
        }

        // Check if user already has a review for this book
        Review? existingReview = await _reviewRepository.GetByBookAndUserAsync(request.BookId, userId, cancellationToken);
        if (existingReview is not null)
        {
            return Result.Failure<Guid>(ReviewErrors.AlreadyExists);
        }

        // Create comment
        Result<Comment> commentResult = new Comment(request.Comment);
        if (commentResult.IsFailure)
        {
            return Result.Failure<Guid>(commentResult.Error);
        }

        // Create rating if provided
        Rating? rating = null;
        if (request.Rating.HasValue)
        {
            Result<Rating> ratingResult = Rating.Create(request.Rating.Value);
            if (ratingResult.IsFailure)
            {
                return Result.Failure<Guid>(ratingResult.Error);
            }
            rating = ratingResult.Value;
        }

        // Create review
        var reviewId = Guid.NewGuid();
        Result<Review> reviewResult = Review.Create(
            reviewId,
            book,
            userId,
            commentResult.Value,
            rating,
            _dateTimeProvider.UtcNow);

        if (reviewResult.IsFailure)
        {
            return Result.Failure<Guid>(reviewResult.Error);
        }

        _reviewRepository.Add(reviewResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(reviewId);
    }
}
