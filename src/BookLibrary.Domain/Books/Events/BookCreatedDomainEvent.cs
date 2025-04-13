using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books.Events;

public record BookCreatedDomainEvent(Guid Id, BookTitle Title, Author Author, int Quantity, Guid LibraryId) : IDomainEvent;