using FluentValidation;

namespace BookLibrary.Application.Reviews.CreateReview;

internal sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(c => c.BookId)
            .NotEmpty()
            .WithMessage("Book ID is required.");

        RuleFor(c => c.Comment)
            .NotEmpty()
            .WithMessage("Comment is required.")
            .MaximumLength(1000)
            .WithMessage("Comment must not exceed 1000 characters.");

        RuleFor(c => c.Rating)
            .InclusiveBetween(1, 5)
            .When(c => c.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5.");
    }
}
