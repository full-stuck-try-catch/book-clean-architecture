using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books.Events;

public record BookBorrowedDomainEvent(Book Book) : IDomainEvent;