using FluentValidation;

namespace BookLibrary.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidatorTest : AbstractValidator<RegisterUserCommandForTest>
{
    public RegisterUserCommandValidatorTest()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(400);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
