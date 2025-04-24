using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books.Events;

public sealed record BookBorrowedDomainEvent(Book Book) : IDomainEvent;
