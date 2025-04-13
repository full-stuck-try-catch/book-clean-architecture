using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public record LoanExtendedDomainEvent(Loan Loan, LoanPeriod Extension) : IDomainEvent;
