using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Domain.Users.Events;

public record BookReturnedDomainEvent(User User, Book Book, Loan Loan) : IDomainEvent;