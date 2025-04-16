using Asp.Versioning;
using BookLibrary.Application.Loans.CreateLoan;
using BookLibrary.Application.Loans.ReturnLoan;
using BookLibrary.Application.Loans.ExtendLoan;
using BookLibrary.Application.Loans.GetLoan;
using BookLibrary.Application.Loans.GetUserLoans;
using BookLibrary.Application.Loans.GetUserActiveLoans;
using BookLibrary.Application.Loans.GetAllActiveLoans;
using BookLibrary.Application.Loans.GetOverdueLoans;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Loans;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ISender _sender;

    public LoansController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan(
        CreateLoanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLoanCommand(
            request.BookId,
            request.StartDate,
            request.EndDate);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetLoan),
            new { loanId = result.Value },
            result.Value);
    }

    [HttpGet("{loanId:guid}")]
    public async Task<IActionResult> GetLoan(
        Guid loanId,
        CancellationToken cancellationToken)
    {
        var query = new GetLoanQuery(loanId);

        Result<LoanResponse> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{loanId:guid}/return")]
    public async Task<IActionResult> ReturnLoan(
        Guid loanId,
        ReturnLoanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReturnLoanCommand(loanId, request.ReturnedAt);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{loanId:guid}/extend")]
    public async Task<IActionResult> ExtendLoan(
        Guid loanId,
        ExtendLoanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ExtendLoanCommand(loanId, request.NewEndDate);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpGet("my-loans")]
    public async Task<IActionResult> GetMyLoans(CancellationToken cancellationToken)
    {
        var query = new GetUserLoansQuery();

        Result<List<UsersLoanResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("my-active-loans")]
    public async Task<IActionResult> GetMyActiveLoans(CancellationToken cancellationToken)
    {
        var query = new GetUserActiveLoansQuery();

        Result<List<LoanUserActiveResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("active")]
    [HasPermission("loans:read")]
    public async Task<IActionResult> GetAllActiveLoans(CancellationToken cancellationToken)
    {
        var query = new GetAllActiveLoansQuery();

        Result<List<LoanActiveResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("overdue")]
    [HasPermission("loans:read")]
    public async Task<IActionResult> GetOverdueLoans(CancellationToken cancellationToken)
    {
        var query = new GetOverdueLoansQuery();

        Result<List<LoanOverdueResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
