using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Reviews;
using BookLibrary.Domain.Reviews.Events;
using BookLibrary.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace BookLibrary.Domain.UnitTests.Reviews;

public class ReviewTests : BaseTest
{
    [Fact]
    public void Create_WithAvailableBook_Should_Succeed()
    {
        // Arrange
        Guid id = ReviewData.GetTestGuid();
        Book book = ReviewData.CreateAvailableBook();
        Guid userId = ReviewData.TestUserId;
        Comment comment = ReviewData.TestComment;
        Rating rating = ReviewData.TestRating;
        DateTime createdAt = ReviewData.TestCreationDate;

        // Act
        var result = Review.Create(id, book, userId, comment, rating, createdAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var review = result.Value;
        review.Id.Should().Be(id);
        review.BookId.Should().Be(book.Id);
        review.UserId.Should().Be(userId);
        review.Comment.Should().Be(comment);
        review.Rating.Should().Be(rating);
        review.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Create_WithAvailableBook_Should_RaiseReviewCreatedDomainEvent()
    {
        // Arrange
        Guid id = ReviewData.GetTestGuid();
        Book book = ReviewData.CreateAvailableBook();
        Guid userId = ReviewData.TestUserId;
        Comment comment = ReviewData.TestComment;
        Rating rating = ReviewData.TestRating;
        DateTime createdAt = ReviewData.TestCreationDate;

        // Act
        var review = Review.Create(id, book, userId, comment, rating, createdAt).Value;

        // Assert
        ReviewCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<ReviewCreatedDomainEvent>(review);
        domainEvent.Id.Should().Be(id);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.Comment.Should().Be(comment.Value);
        domainEvent.Rating.Should().Be(rating.Value);
    }

    [Fact]
    public void Create_WithDeletedBook_Should_Fail()
    {
        // Arrange
        Guid id = ReviewData.GetTestGuid();
        Book book = ReviewData.CreateDeletedBook();
        Guid userId = ReviewData.TestUserId;
        Comment comment = ReviewData.TestComment;
        Rating rating = ReviewData.TestRating;
        DateTime createdAt = ReviewData.TestCreationDate;

        // Act
        var result = Review.Create(id, book, userId, comment, rating, createdAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.BookNotAvailable);
    }

    [Fact]
    public void Update_Should_Succeed()
    {
        // Arrange
        Review review = ReviewData.CreateTestReview();
        Comment newComment = new("This is an updated comment.");
        Rating newRating = Rating.Create(5).Value;
        DateTime updatedAt = DateTime.UtcNow;

        // Act
        Result result = review.Update(newComment, newRating, updatedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Comment.Should().Be(newComment);
        review.Rating.Should().Be(newRating);
        review.UpdatedAt.Should().Be(updatedAt);
    }
}
