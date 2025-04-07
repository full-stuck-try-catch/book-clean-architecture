using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards.Events;

public record LibraryCardBlockedDomainEvent(LibraryCard LibraryCard) : IDomainEvent;