using System.Collections.ObjectModel;

namespace BookLibrary.Domain.Abstractions;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }
}