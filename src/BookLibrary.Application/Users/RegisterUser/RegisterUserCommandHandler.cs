using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Validate password length
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return Result.Failure<Guid>(UserErrors.PasswordTooShort);
        }

        // Validate email format
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return Result.Failure<Guid>(UserErrors.InvalidEmailFormat);
        }

        // Check if email is unique
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken);
        if (!isEmailUnique)
        {
            return Result.Failure<Guid>(UserErrors.EmailAlreadyInUse);
        }

        // Create value objects
        var email = Email.Create(request.Email);
        var firstName = new FirstName(request.FirstName);
        var lastName = new LastName(request.LastName);

        // Hash password
        string passwordHash = _passwordHasher.Hash(request.Password);

        // Create user
        var user = User.Create(
            Guid.NewGuid(),
            email,
            firstName,
            lastName,
            new PasswordHash(passwordHash),
            _dateTimeProvider.UtcNow);

        // Add to repository
        _userRepository.Add(user);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
