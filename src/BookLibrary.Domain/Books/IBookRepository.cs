using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Books;

public interface IBookRepository
{
    Task<Book> GetByIdAsync(Guid id);

    ValueTask<bool> ExistsAsync(BookTitle title, AuthorName author);

    void Add(Book book);

    void Delete(Book book);
}