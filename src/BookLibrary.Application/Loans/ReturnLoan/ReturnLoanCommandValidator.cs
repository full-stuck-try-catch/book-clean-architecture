using FluentValidation;

namespace BookLibrary.Application.Loans.ReturnLoan;

internal sealed class ReturnLoanCommandValidator : AbstractValidator<ReturnLoanCommand>
{
    public ReturnLoanCommandValidator()
    {
        RuleFor(c => c.LoanId)
            .NotEmpty();

        RuleFor(c => c.ReturnedAt)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddHours(1))
            .WithMessage("Return date cannot be more than 1 hour in the future");
    }
}
