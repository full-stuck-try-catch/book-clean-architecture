using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public sealed record LoanCreatedDomainEvent(Loan Loan) : IDomainEvent;
