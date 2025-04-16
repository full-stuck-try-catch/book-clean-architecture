using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.ReturnLoan;

public sealed record ReturnLoanCommand(
    Guid LoanId,
    DateTime ReturnedAt) : ICommand;
