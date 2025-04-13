using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Users.GetUserLogged;

public sealed record GetLoggedUserQuery : IQuery<UserResponse>;
