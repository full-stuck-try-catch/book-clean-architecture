using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.GetOverdueLoans;

public sealed record GetOverdueLoansQuery() : IQuery<List<LoanOverdueResponse>>;
