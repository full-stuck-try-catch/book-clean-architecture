using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Loans.Events;

public record LoanCreatedDomainEvent(Loan Loan) : IDomainEvent;