using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLibrary.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;
internal abstract class Repository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public virtual void Add(T entity)
    {
        DbContext.Add(entity);
    }

    public virtual void Update(T entity)
    {
        DbContext.Update(entity);
    }
}
