using BookLibrary.Domain.Libraries;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

internal sealed class LibraryRepository : Repository<Library>, ILibraryRepository
{
    public LibraryRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async ValueTask<bool> ExistsAsync(LibraryName libraryName, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Library>()
            .AnyAsync(library => library.Name == libraryName, cancellationToken);
    }
}
