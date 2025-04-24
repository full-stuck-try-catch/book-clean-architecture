using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public sealed record LoanExtendedDomainEvent(Loan Loan, LoanPeriod Extension) : IDomainEvent;
