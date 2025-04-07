using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.LibraryCards;

public interface ILibraryCardRepository
{
    Task<LibraryCard> GetByIdAsync(Guid id);

    ValueTask<bool> ExistsAsync(LibraryCardNumber cardNumber);

    void Add(LibraryCard libraryCard);

    void Delete(LibraryCard libraryCard);
}