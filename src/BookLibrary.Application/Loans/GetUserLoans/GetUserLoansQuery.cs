using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.GetUserLoans;

public sealed record GetUserLoansQuery() : IQuery<List<UsersLoanResponse>>;
