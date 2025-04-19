using FluentValidation;

namespace BookLibrary.Application.Books.ReturnBook;

public sealed class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Book ID is required.");
    }
}
