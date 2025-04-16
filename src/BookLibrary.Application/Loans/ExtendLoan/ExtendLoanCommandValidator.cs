using FluentValidation;

namespace BookLibrary.Application.Loans.ExtendLoan;

internal sealed class ExtendLoanCommandValidator : AbstractValidator<ExtendLoanCommand>
{
    public ExtendLoanCommandValidator()
    {
        RuleFor(c => c.LoanId)
            .NotEmpty();

        RuleFor(c => c.NewEndDate)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("New end date must be in the future");

        RuleFor(c => c.NewEndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(60))
            .WithMessage("Loan extension cannot exceed 60 days from today");
    }
}
