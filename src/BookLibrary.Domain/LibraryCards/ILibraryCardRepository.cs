using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards;

public interface ILibraryCardRepository
{
    Task<LibraryCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    ValueTask<bool> ExistsAsync(string cardNumber);

    void Add(LibraryCard libraryCard);

    void Update(LibraryCard libraryCard);
}
