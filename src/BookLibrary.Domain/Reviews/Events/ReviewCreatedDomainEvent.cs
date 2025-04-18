using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(Guid Id , Guid UserId , string Comment , int? Rating) : IDomainEvent; 
