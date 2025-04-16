using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.GetUserActiveLoans;

internal sealed class GetUserActiveLoansQueryHandler : IQueryHandler<GetUserActiveLoansQuery, List<LoanUserActiveResponse>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserContext _userContext;

    public GetUserActiveLoansQueryHandler(
        ILoanRepository loanRepository,
        IUserContext userContext)
    {
        _loanRepository = loanRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<LoanUserActiveResponse>>> Handle(GetUserActiveLoansQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Get active loans for the current user
        List<Loan> loans = await _loanRepository.GetActiveLoansByUserIdAsync(userId, cancellationToken);

        var response = loans.Select(loan => new LoanUserActiveResponse(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.Period.StartDate,
            loan.Period.EndDate,
            loan.ReturnedAt))
            .ToList();

        return Result.Success(response);
    }
}
