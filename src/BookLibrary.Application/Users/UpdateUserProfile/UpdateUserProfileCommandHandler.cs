using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Users.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Find user by ID
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        // Create value objects
        var firstName = new FirstName(request.FirstName);
        var lastName = new LastName(request.LastName);

        // Update user
        Result updateResult = user.Update(
            firstName,
            lastName,
            _dateTimeProvider.UtcNow);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        // Update repository
        _userRepository.Update(user);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
