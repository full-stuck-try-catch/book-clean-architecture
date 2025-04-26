using Asp.Versioning;
using BookLibrary.Application.Libraries.AddBookToLibrary;
using BookLibrary.Application.Libraries.CreateLibrary;
using BookLibrary.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Libraries;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/libraries")]
public class LibrariesController : ControllerBase
{
    private readonly ISender _sender;

    public LibrariesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLibrary(
        CreateLibraryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLibraryCommand(request.Name);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(CreateLibrary),
            new { id = result.Value },
            result.Value);
    }

    [HttpPost("{libraryId:guid}/books")]
    public async Task<IActionResult> AddBookToLibrary(
        Guid libraryId,
        AddBookToLibraryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddBookToLibraryCommand(
            libraryId,
            request.Title,
            request.AuthorFirstName,
            request.AuthorLastName,
            request.AuthorCountry,
            request.Quantity);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(AddBookToLibrary),
            new { libraryId, bookId = result.Value },
            result.Value);
    }
}
