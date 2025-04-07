using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards.Events;

public record LibraryCardLostDomainEvent(LibraryCard LibraryCard) : IDomainEvent;