using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Users.RegisterUser;

public sealed record RegisterUserCommandForTest(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Password) : ICommand<Guid>;
