using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public sealed record LoanReturnedDomainEvent(Loan Loan) : IDomainEvent;
