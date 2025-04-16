using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.GetLoan;

internal sealed class GetLoanQueryHandler : IQueryHandler<GetLoanQuery, LoanResponse>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserContext _userContext;

    public GetLoanQueryHandler(
        ILoanRepository loanRepository,
        IUserContext userContext)
    {
        _loanRepository = loanRepository;
        _userContext = userContext;
    }

    public async Task<Result<LoanResponse>> Handle(GetLoanQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        Loan? loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);

        if (loan is null)
        {
            return Result.Failure<LoanResponse>(LoanErrors.NotFound);
        }

        // Ensure the loan belongs to the current user
        if (loan.UserId != userId)
        {
            return Result.Failure<LoanResponse>(LoanErrors.NotFound); // Return NotFound for security reasons
        }

        var response = new LoanResponse(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.Period.StartDate,
            loan.Period.EndDate,
            loan.ReturnedAt);

        return Result.Success(response);
    }
}
