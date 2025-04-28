using BookLibrary.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

internal sealed class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<Book>> GetByLibraryIdAsync(Guid libraryId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Book>()
            .Where(book => book.LibraryId == libraryId)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(BookTitle title, Author author, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Book>()
            .AnyAsync(book => book.Title == title && 
                             book.Author.AuthorFirstName == author.AuthorFirstName && book.Author.AuthorLastName == author.AuthorLastName
                             && book.Author.AuthorCountry == author.AuthorCountry, 
                      cancellationToken);
    }
}
