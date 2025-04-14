using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Domain.Users.Events;

public sealed record BookBorrowedDomainEvent(Guid UserId, Book Book, Loan Loan) : IDomainEvent;
