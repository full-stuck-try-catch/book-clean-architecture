using Asp.Versioning;
using BookLibrary.Application.Reviews.CreateReview;
using BookLibrary.Application.Reviews.DeleteReview;
using BookLibrary.Application.Reviews.GetBookReviews;
using BookLibrary.Application.Reviews.GetReview;
using BookLibrary.Application.Reviews.UpdateReview;
using BookLibrary.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Reviews;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public ReviewsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(
        CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(
            request.BookId,
            request.Comment,
            request.Rating);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetReview),
            new { reviewId = result.Value },
            result.Value);
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetReview(
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var query = new GetReviewQuery(reviewId);

        Result<ReviewResponse> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(
        Guid reviewId,
        UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReviewCommand(
            reviewId,
            request.Comment,
            request.Rating);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteReviewCommand(reviewId);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpGet("books/{bookId:guid}")]
    public async Task<IActionResult> GetBookReviews(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var query = new GetBookReviewsQuery(bookId);

        Result<List<ReviewResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
