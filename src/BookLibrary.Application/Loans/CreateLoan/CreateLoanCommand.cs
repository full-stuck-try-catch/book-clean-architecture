using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Loans.CreateLoan;

public sealed record CreateLoanCommand(
    Guid BookId,
    DateTime StartDate,
    DateTime EndDate) : ICommand<Guid>;
