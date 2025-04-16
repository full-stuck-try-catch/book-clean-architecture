using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.GetLoan;

public sealed record GetLoanQuery(Guid LoanId) : IQuery<LoanResponse>;
