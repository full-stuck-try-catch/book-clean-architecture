using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Domain.Libraries.Events;

public sealed record BookRemovedFromLibraryDomainEvent(Book Book) : IDomainEvent;
