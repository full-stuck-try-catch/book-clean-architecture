using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.GetAllActiveLoans;

internal sealed class GetAllActiveLoansQueryHandler : IQueryHandler<GetAllActiveLoansQuery, List<LoanActiveResponse>>
{
    private readonly ILoanRepository _loanRepository;

    public GetAllActiveLoansQueryHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<Result<List<LoanActiveResponse>>> Handle(GetAllActiveLoansQuery request, CancellationToken cancellationToken)
    {
        // Get all active loans (admin functionality)
        List<Loan> loans = await _loanRepository.GetActiveLoansAsync(cancellationToken);

        var response = loans.Select(loan => new LoanActiveResponse(
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
