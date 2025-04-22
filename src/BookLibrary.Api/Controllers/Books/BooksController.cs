using Asp.Versioning;
using BookLibrary.Application.Books.AddStock;
using BookLibrary.Application.Books.CreateBook;
using BookLibrary.Application.Books.GetBook;
using BookLibrary.Application.Books.GetBooksByLibrary;
using BookLibrary.Application.Books.MarkBookAsBorrowed;
using BookLibrary.Application.Books.MarkBookAsDeleted;
using BookLibrary.Application.Books.MarkBookAsReturned;
using BookLibrary.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Books;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/books")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly ISender _sender;

    public BooksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(
        CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(
            request.Title,
            request.AuthorFirstName,
            request.AuthorLastName,
            request.AuthorCountry,
            request.Quantity,
            request.LibraryId);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetBook),
            new { bookId = result.Value },
            result.Value);
    }

    [HttpGet("{bookId:guid}")]
    public async Task<IActionResult> GetBook(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var query = new GetBookQuery(bookId);

        Result<BookResponse> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("library/{libraryId:guid}")]
    public async Task<IActionResult> GetBooksByLibrary(
        Guid libraryId,
        CancellationToken cancellationToken)
    {
        var query = new GetBooksByLibraryQuery(libraryId);

        Result<IReadOnlyList<BookResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{bookId:guid}/add-stock")]
    public async Task<IActionResult> AddStock(
        Guid bookId,
        AddStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddStockCommand(bookId, request.Count);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{bookId:guid}/mark-borrowed")]
    public async Task<IActionResult> MarkBookAsBorrowed(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var command = new MarkBookAsBorrowedCommand(bookId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{bookId:guid}/mark-returned")]
    public async Task<IActionResult> MarkBookAsReturned(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var command = new MarkBookAsReturnedCommand(bookId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{bookId:guid}")]
    public async Task<IActionResult> MarkBookAsDeleted(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var command = new MarkBookAsDeletedCommand(bookId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
