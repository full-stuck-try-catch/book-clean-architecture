using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public record LoanReturnedDomainEvent(Loan Loan) : IDomainEvent;