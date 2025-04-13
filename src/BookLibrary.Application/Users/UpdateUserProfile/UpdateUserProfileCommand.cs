using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Users.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName) : ICommand;
