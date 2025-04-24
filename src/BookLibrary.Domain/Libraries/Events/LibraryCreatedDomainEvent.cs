using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries.Events;

public sealed record LibraryCreatedDomainEvent(Library Library) : IDomainEvent;
