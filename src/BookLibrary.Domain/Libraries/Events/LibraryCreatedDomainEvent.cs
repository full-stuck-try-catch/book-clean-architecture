using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries.Events;

public record LibraryCreatedDomainEvent(Library Library) : IDomainEvent;