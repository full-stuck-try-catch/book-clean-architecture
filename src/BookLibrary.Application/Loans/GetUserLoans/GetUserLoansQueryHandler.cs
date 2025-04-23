using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.GetUserLoans;

public sealed class GetUserLoansQueryHandler : IQueryHandler<GetUserLoansQuery, List<UsersLoanResponse>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserContext _userContext;

    public GetUserLoansQueryHandler(
        ILoanRepository loanRepository,
        IUserContext userContext)
    {
        _loanRepository = loanRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<UsersLoanResponse>>> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID from context
        Guid userId = _userContext.UserId;

        // Get all loans for the current user
        List<Loan> loans = await _loanRepository.GetByUserIdAsync(userId, cancellationToken);

        var response = loans.Select(loan => new UsersLoanResponse(
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
