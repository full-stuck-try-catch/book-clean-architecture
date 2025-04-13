using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Abstractions.Data;
using BookLibrary.Application.Exceptions;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Reviews;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookLibrary.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork, IApplicationDbContext
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<Library> Libraries { get; private set; }
    public DbSet<Book> Books { get; private set; }
    public DbSet<Loan> Loans { get; private set; }
    public DbSet<User> Users { get; private set; }
    public DbSet<Role> Roles { get; private set; }
    public DbSet<Review> Reviews { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }
}
