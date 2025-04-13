using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Loans;

namespace BookLibrary.Domain.Users.Events;

public record BookReturnedDomainEvent(Guid UserId, Guid BookId, DateTime ReturnedAt) : IDomainEvent;