using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Reviews.CreateReview;
using BookLibrary.Application.Reviews.UpdateReview;
using BookLibrary.Application.Reviews.DeleteReview;
using BookLibrary.Application.Reviews.GetReview;
using BookLibrary.Application.Reviews.GetBookReviews;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Reviews;
using FluentAssertions;
using NSubstitute;

namespace BookLibrary.Application.UnitTests.Reviews;

public class ReviewTests
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    // Command handlers
    private readonly CreateReviewCommandHandler _createReviewCommandHandler;
    private readonly UpdateReviewCommandHandler _updateReviewCommandHandler;
    private readonly DeleteReviewCommandHandler _deleteReviewCommandHandler;

    // Query handlers
    private readonly GetReviewQueryHandler _getReviewQueryHandler;
    private readonly GetBookReviewsQueryHandler _getBookReviewsQueryHandler;

    // Test data
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _bookId = Guid.NewGuid();
    private readonly Guid _reviewId = Guid.NewGuid();
    private readonly DateTime _testDateTime = DateTime.UtcNow;

    public ReviewTests()
    {
        _reviewRepository = Substitute.For<IReviewRepository>();
        _bookRepository = Substitute.For<IBookRepository>();
        _userContext = Substitute.For<IUserContext>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        // Initialize command handlers
        _createReviewCommandHandler = new CreateReviewCommandHandler(
            _reviewRepository,
            _bookRepository,
            _userContext,
            _dateTimeProvider,
            _unitOfWork);

        _updateReviewCommandHandler = new UpdateReviewCommandHandler(
            _reviewRepository,
            _userContext,
            _dateTimeProvider,
            _unitOfWork);

        _deleteReviewCommandHandler = new DeleteReviewCommandHandler(
            _reviewRepository,
            _userContext,
            _unitOfWork);

        // Initialize query handlers
        _getReviewQueryHandler = new GetReviewQueryHandler(
            _reviewRepository,
            _userContext);

        _getBookReviewsQueryHandler = new GetBookReviewsQueryHandler(
            _reviewRepository);

        // Setup common mocks
        _userContext.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(_testDateTime);
    }

    #region CreateReviewCommandHandler Tests

    [Fact]
    public async Task CreateReview_WithValidData_Should_Succeed()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", 5);
        Book book = CreateTestBook(_bookId);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _reviewRepository.GetByBookAndUserAsync(_bookId, _userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _reviewRepository.Received(1).Add(Arg.Any<Review>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReview_WithValidDataWithoutRating_Should_Succeed()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", null);
        Book book = CreateTestBook(_bookId);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _reviewRepository.GetByBookAndUserAsync(_bookId, _userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _reviewRepository.Received(1).Add(Arg.Any<Review>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReview_WithNonExistentBook_Should_ReturnBookNotFoundError()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", 5);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NotFound);
        _reviewRepository.DidNotReceive().Add(Arg.Any<Review>());
    }

    [Fact]
    public async Task CreateReview_WithExistingReview_Should_ReturnReviewAlreadyExistsError()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", 5);
        Book book = CreateTestBook(_bookId);
        Review existingReview = CreateTestReview(_reviewId, _userId, _bookId);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _reviewRepository.GetByBookAndUserAsync(_bookId, _userId, Arg.Any<CancellationToken>())
            .Returns(existingReview);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.AlreadyExists);
        _reviewRepository.DidNotReceive().Add(Arg.Any<Review>());
    }

    [Fact]
    public async Task CreateReview_WithInvalidRating_Should_ReturnRatingInvalidError()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", 6); // Invalid rating > 5
        Book book = CreateTestBook(_bookId);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _reviewRepository.GetByBookAndUserAsync(_bookId, _userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Rating.Invalid);
        _reviewRepository.DidNotReceive().Add(Arg.Any<Review>());
    }

    [Fact]
    public async Task CreateReview_WithDeletedBook_Should_ReturnBookNotAvailableError()
    {
        // Arrange
        var command = new CreateReviewCommand(_bookId, "Great book!", 5);
        Book book = CreateDeletedBook(_bookId);

        _bookRepository.GetByIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(book);
        _reviewRepository.GetByBookAndUserAsync(_bookId, _userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result<Guid> result = await _createReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.BookNotAvailable);
        _reviewRepository.DidNotReceive().Add(Arg.Any<Review>());
    }

    #endregion

    #region UpdateReviewCommandHandler Tests

    [Fact]
    public async Task UpdateReview_WithValidData_Should_Succeed()
    {
        // Arrange
        var command = new UpdateReviewCommand(_reviewId, "Updated comment", 4);
        Review review = CreateTestReview(_reviewId, _userId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepository.Received(1).Update(review);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReview_WithValidDataWithoutRating_Should_Succeed()
    {
        // Arrange
        var command = new UpdateReviewCommand(_reviewId, "Updated comment", null);
        Review review = CreateTestReview(_reviewId, _userId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepository.Received(1).Update(review);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReview_WithNonExistentReview_Should_ReturnReviewNotFoundError()
    {
        // Arrange
        var command = new UpdateReviewCommand(_reviewId, "Updated comment", 4);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotFound);
        _reviewRepository.DidNotReceive().Update(Arg.Any<Review>());
    }

    [Fact]
    public async Task UpdateReview_WithReviewNotBelongingToUser_Should_ReturnUnauthorizedError()
    {
        // Arrange
        var command = new UpdateReviewCommand(_reviewId, "Updated comment", 4);
        var otherUserId = Guid.NewGuid();
        Review review = CreateTestReview(_reviewId, otherUserId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.Unauthorized);
        _reviewRepository.DidNotReceive().Update(Arg.Any<Review>());
    }

    [Fact]
    public async Task UpdateReview_WithInvalidRating_Should_ReturnRatingInvalidError()
    {
        // Arrange
        var command = new UpdateReviewCommand(_reviewId, "Updated comment", 0); // Invalid rating < 1
        Review review = CreateTestReview(_reviewId, _userId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Rating.Invalid);
        _reviewRepository.DidNotReceive().Update(Arg.Any<Review>());
    }

    #endregion

    #region DeleteReviewCommandHandler Tests

    [Fact]
    public async Task DeleteReview_WithValidData_Should_Succeed()
    {
        // Arrange
        var command = new DeleteReviewCommand(_reviewId);
        Review review = CreateTestReview(_reviewId, _userId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _deleteReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepository.Received(1).Remove(review);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReview_WithNonExistentReview_Should_ReturnReviewNotFoundError()
    {
        // Arrange
        var command = new DeleteReviewCommand(_reviewId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result result = await _deleteReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotFound);
        _reviewRepository.DidNotReceive().Remove(Arg.Any<Review>());
    }

    [Fact]
    public async Task DeleteReview_WithReviewNotBelongingToUser_Should_ReturnUnauthorizedError()
    {
        // Arrange
        var command = new DeleteReviewCommand(_reviewId);
        var otherUserId = Guid.NewGuid();
        Review review = CreateTestReview(_reviewId, otherUserId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _deleteReviewCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.Unauthorized);
        _reviewRepository.DidNotReceive().Remove(Arg.Any<Review>());
    }

    #endregion

    #region GetReviewQueryHandler Tests

    [Fact]
    public async Task GetReview_WithValidData_Should_ReturnReviewResponse()
    {
        // Arrange
        var query = new GetReviewQuery(_reviewId);
        Review review = CreateTestReview(_reviewId, _userId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result<ReviewResponse> result = await _getReviewQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_reviewId);
        result.Value.BookId.Should().Be(_bookId);
        result.Value.UserId.Should().Be(_userId);
        result.Value.Comment.Should().Be(review.Comment.Value);
        result.Value.Rating.Should().Be(review.Rating?.Value);
        result.Value.CreatedAt.Should().Be(review.CreatedAt);
        result.Value.UpdatedAt.Should().Be(review.UpdatedAt);
    }

    [Fact]
    public async Task GetReview_WithNonExistentReview_Should_ReturnReviewNotFoundError()
    {
        // Arrange
        var query = new GetReviewQuery(_reviewId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        Result<ReviewResponse> result = await _getReviewQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotFound);
    }

    [Fact]
    public async Task GetReview_WithReviewNotBelongingToUser_Should_ReturnUnauthorizedError()
    {
        // Arrange
        var query = new GetReviewQuery(_reviewId);
        var otherUserId = Guid.NewGuid();
        Review review = CreateTestReview(_reviewId, otherUserId, _bookId);

        _reviewRepository.GetByIdAsync(_reviewId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result<ReviewResponse> result = await _getReviewQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.Unauthorized);
    }

    #endregion

    #region GetBookReviewsQueryHandler Tests

    [Fact]
    public async Task GetBookReviews_WithValidData_Should_ReturnReviewResponses()
    {
        // Arrange
        var query = new GetBookReviewsQuery(_bookId);
        Review review1 = CreateTestReview(Guid.NewGuid(), _userId, _bookId);
        Review review2 = CreateTestReview(Guid.NewGuid(), Guid.NewGuid(), _bookId);
        var reviews = new List<Review> { review1, review2 };

        _reviewRepository.GetByBookIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(reviews);

        // Act
        Result<List<ReviewResponse>> result = await _getBookReviewsQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);

        ReviewResponse firstReview = result.Value.First(r => r.Id == review1.Id);
        firstReview.BookId.Should().Be(_bookId);
        firstReview.UserId.Should().Be(review1.UserId);
        firstReview.Comment.Should().Be(review1.Comment.Value);
        firstReview.Rating.Should().Be(review1.Rating?.Value);
        firstReview.CreatedAt.Should().Be(review1.CreatedAt);
        firstReview.UpdatedAt.Should().Be(review1.UpdatedAt);
    }

    [Fact]
    public async Task GetBookReviews_WithNoReviews_Should_ReturnEmptyList()
    {
        // Arrange
        var query = new GetBookReviewsQuery(_bookId);
        var reviews = new List<Review>();

        _reviewRepository.GetByBookIdAsync(_bookId, Arg.Any<CancellationToken>())
            .Returns(reviews);

        // Act
        Result<List<ReviewResponse>> result = await _getBookReviewsQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Book CreateTestBook(Guid bookId)
    {
        var bookTitle = new BookTitle("Test Book");
        var author = new Author("John", "Doe", "USA");
        return Book.Create(bookId, bookTitle, author, 1, Guid.NewGuid());
    }

    private static Book CreateDeletedBook(Guid bookId)
    {
        var bookTitle = new BookTitle("Deleted Book");
        var author = new Author("Jane", "Smith", "UK");
        var book = Book.Create(bookId, bookTitle, author, 0, Guid.NewGuid());
        book.MarkAsDeleted();
        return book;
    }

    private static Review CreateTestReview(Guid reviewId, Guid userId, Guid bookId)
    {
        Book book = CreateTestBook(bookId);
        var comment = new Comment("This is a test review");
        Rating rating = Rating.Create(4).Value;
        DateTime createdAt = DateTime.UtcNow;

        Review review = Review.Create(reviewId, book, userId, comment, rating, createdAt).Value;
        
        // Clear domain events to avoid side effects in tests
        review.ClearDomainEvents();
        
        return review;
    }

    #endregion
}
