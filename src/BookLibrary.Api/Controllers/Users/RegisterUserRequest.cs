namespace BookLibrary.Api.Controllers.Users;

public sealed record RegisterUserRequest(
    string firstName,
    string lastName,
    string email,
    string password);
