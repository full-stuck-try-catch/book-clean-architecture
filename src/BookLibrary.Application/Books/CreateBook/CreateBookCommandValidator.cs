using FluentValidation;

namespace BookLibrary.Application.Books.CreateBook;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Book title is required and should not exceed 500 characters.");

        RuleFor(c => c.AuthorFirstName)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Author first name is required and should not exceed 200 characters.");

        RuleFor(c => c.AuthorLastName)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Author last name is required and should not exceed 200 characters.");

        RuleFor(c => c.AuthorCountry)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Author country is required and should not exceed 200 characters.");

        RuleFor(c => c.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(c => c.LibraryId)
            .NotEmpty()
            .WithMessage("Library ID is required.");
    }
}
