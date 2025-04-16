using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.GetAllActiveLoans;

public sealed record GetAllActiveLoansQuery() : IQuery<List<LoanActiveResponse>>;
