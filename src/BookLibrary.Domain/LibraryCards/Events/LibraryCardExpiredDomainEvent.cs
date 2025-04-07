using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards.Events;

public record LibraryCardExpiredDomainEvent(LibraryCard LibraryCard) : IDomainEvent;