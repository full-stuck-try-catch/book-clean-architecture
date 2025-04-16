using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.GetUserActiveLoans;

public sealed record GetUserActiveLoansQuery() : IQuery<List<LoanUserActiveResponse>>;
