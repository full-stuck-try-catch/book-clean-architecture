using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards.Events;

public record LibraryCardCreatedDomainEvent(LibraryCard LibraryCard) : IDomainEvent;
