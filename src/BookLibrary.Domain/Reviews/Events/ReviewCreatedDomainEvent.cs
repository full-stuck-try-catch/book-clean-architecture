using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(Review Review) : IDomainEvent; 