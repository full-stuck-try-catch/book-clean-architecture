using FluentValidation;

namespace BookLibrary.Application.Libraries.CreateLibrary;

internal sealed class CreateLibraryCommandValidator : AbstractValidator<CreateLibraryCommand>
{
    public CreateLibraryCommandValidator()
    {
        RuleFor(c => c.Name.Value)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Library name must not be empty and should not exceed 200 characters.");
    }
}
