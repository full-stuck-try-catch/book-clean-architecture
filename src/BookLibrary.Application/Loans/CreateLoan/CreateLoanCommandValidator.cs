using FluentValidation;

namespace BookLibrary.Application.Loans.CreateLoan;

internal sealed class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
{
    public CreateLoanCommandValidator()
    {
        RuleFor(c => c.BookId)
            .NotEmpty();

        RuleFor(c => c.StartDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Start date cannot be more than 1 day in the future");

        RuleFor(c => c.EndDate)
            .NotEmpty()
            .GreaterThan(c => c.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(c => c.EndDate)
            .LessThanOrEqualTo(c => c.StartDate.AddDays(30))
            .WithMessage("Loan period cannot exceed 30 days");
    }
}
