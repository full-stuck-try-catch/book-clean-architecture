using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Book>> GetByLibraryIdAsync(Guid libraryId, CancellationToken cancellationToken);

    ValueTask<bool> ExistsAsync(BookTitle title, Author author , CancellationToken cancellationToken);

    void Add(Book book);

    void Update(Book book);
}
