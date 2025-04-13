using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Users.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<AccessTokenResponse>;
