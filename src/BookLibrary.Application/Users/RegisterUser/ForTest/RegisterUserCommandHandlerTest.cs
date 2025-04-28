using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Data;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandlerForTest : ICommandHandler<RegisterUserCommandForTest, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandlerForTest(
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

    public async Task<Result<Guid>> Handle(RegisterUserCommandForTest request, CancellationToken cancellationToken)
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
        var email =new Email(request.Email);
        var firstName = new FirstName(request.FirstName);
        var lastName = new LastName(request.LastName);

        // Hash password
        string passwordHash = _passwordHasher.Hash(request.Password);

        // Create user
        var user = User.Create(
            request.UserId,
            email,
            firstName,
            lastName,
            new PasswordHash(passwordHash),
            _dateTimeProvider.UtcNow);

        Role? userRole = await _userRepository.GetUserRole(Role.User.Name, cancellationToken);

        user.AddRole(userRole);

        // Add to repository
        _userRepository.Add(user);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {

            throw e;
        }
        // Save changes
        

        return Result.Success(user.Id);
    }
}
