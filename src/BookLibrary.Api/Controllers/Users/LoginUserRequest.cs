﻿namespace BookLibrary.Api.Controllers.Users;

public sealed record LoginUserRequest(string Email,
    string Password
);
