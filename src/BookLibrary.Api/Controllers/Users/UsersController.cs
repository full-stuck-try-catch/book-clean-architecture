using Asp.Versioning;
using BookLibrary.Application.Users.GetUserLogged;
using BookLibrary.Application.Users.LoginUser;
using BookLibrary.Application.Users.RegisterUser;
using BookLibrary.Application.Users.UpdateUserProfile;
using BookLibrary.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Users;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<AccessTokenResponse> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetLoggedUser(CancellationToken cancellationToken)
    {
        var query = new GetLoggedUserQuery();

        Result<UserResponse> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateUserProfile(
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfileCommand(
            request.UserId,
            request.FirstName,
            request.LastName);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
