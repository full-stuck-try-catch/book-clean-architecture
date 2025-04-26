using Asp.Versioning;
using BookLibrary.Application.Books.BorrowBook;
using BookLibrary.Application.Books.ReturnBook;
using BookLibrary.Application.Users.GetUserLogged;
using BookLibrary.Application.Users.LoginUser;
using BookLibrary.Application.Users.RegisterUser;
using BookLibrary.Application.Users.UpdateUserProfile;
using BookLibrary.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Users;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.firstName,
            request.lastName,
            request.email,
            request.password);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
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

    [HttpPost("borrow-book")]
    [Authorize]
    public async Task<IActionResult> BorrowBook(
        BorrowBookUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BorrowBookCommand(request.BookId, request.StartDate, request.EndDate);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("return-book")]
    [Authorize]
    public async Task<IActionResult> ReturnBook(
        ReturnBookUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReturnBookCommand(request.BookId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
