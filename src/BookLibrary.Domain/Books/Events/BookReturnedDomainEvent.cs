using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books.Events;

public sealed record BookReturnedDomainEvent(Book Book) : IDomainEvent;
