using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.LibraryCards.Events;
using BookLibrary.Domain.Users;

namespace BookLibrary.Domain.LibraryCards;

public sealed class LibraryCard : Entity
{
    public LibraryCardNumber CardNumber { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public LibraryCardStatus Status { get; private set; }
    public bool IsActive => Status == LibraryCardStatus.Active && 
                            (ExpiredAt == null || ExpiredAt > DateTime.UtcNow);

    private LibraryCard(Guid id, LibraryCardNumber cardNumber, Guid userId, DateTime issuedAt) : base(id)
    {
        CardNumber = cardNumber;
        UserId = userId;
        IssuedAt = issuedAt;
        Status = LibraryCardStatus.Active;
    }

    public static LibraryCard Create(Guid id, LibraryCardNumber cardNumber, Guid userId, DateTime issuedAt)
    {
        var libraryCard = new LibraryCard(id, cardNumber, userId, issuedAt);
        libraryCard.RaiseDomainEvent(new LibraryCardCreatedDomainEvent(libraryCard));
        return libraryCard;
    }

    public void Expire(DateTime expiredAt)
    {
        ExpiredAt = expiredAt;
        Status = LibraryCardStatus.Expired;
        RaiseDomainEvent(new LibraryCardExpiredDomainEvent(this));
    }

    public void Block()
    {
        Status = LibraryCardStatus.Blocked;
        RaiseDomainEvent(new LibraryCardBlockedDomainEvent(this));
    }

    public void MarkLost()
    {
        Status = LibraryCardStatus.Lost;
        RaiseDomainEvent(new LibraryCardLostDomainEvent(this));
    }
}