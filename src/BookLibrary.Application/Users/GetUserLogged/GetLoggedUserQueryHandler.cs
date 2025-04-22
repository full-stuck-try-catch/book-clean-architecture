using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Users.GetUserLogged;

public sealed class GetLoggedUserQueryHandler : IQueryHandler<GetLoggedUserQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetLoggedUserQueryHandler(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<UserResponse>> Handle(GetLoggedUserQuery request, CancellationToken cancellationToken)
    {
        // Get current user from context
        User? user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }

        var response = new UserResponse(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value);

        return Result.Success(response);
    }
}
