using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Application.Loans.GetOverdueLoans;

internal sealed class GetOverdueLoansQueryHandler : IQueryHandler<GetOverdueLoansQuery, List<LoanOverdueResponse>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetOverdueLoansQueryHandler(
        ILoanRepository loanRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _loanRepository = loanRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<List<LoanOverdueResponse>>> Handle(GetOverdueLoansQuery request, CancellationToken cancellationToken)
    {
        // Get overdue loans (admin functionality)
        List<Loan> loans = await _loanRepository.GetOverdueLoansAsync(_dateTimeProvider.UtcNow, cancellationToken);

        var response = loans.Select(loan => new LoanOverdueResponse(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.Period.StartDate,
            loan.Period.EndDate,
            loan.ReturnedAt)) // All these loans are overdue by definition
            .ToList();

        return Result.Success(response);
    }
}
