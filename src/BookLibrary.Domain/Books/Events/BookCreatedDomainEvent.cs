using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books.Events;

public record BookCreatedDomainEvent(Book Book) : IDomainEvent;