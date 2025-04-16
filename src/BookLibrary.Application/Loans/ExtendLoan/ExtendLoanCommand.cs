using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.ExtendLoan;

public sealed record ExtendLoanCommand(
    Guid LoanId,
    DateTime NewEndDate) : ICommand;
