using FluentValidation;

namespace BookLibrary.Application.Books.AddStock;

internal sealed class AddStockCommandValidator : AbstractValidator<AddStockCommand>
{
    public AddStockCommandValidator()
    {
        RuleFor(c => c.BookId)
            .NotEmpty()
            .WithMessage("Book ID is required.");

        RuleFor(c => c.Count)
            .GreaterThan(0)
            .WithMessage("Stock count must be greater than zero.");
    }
}
