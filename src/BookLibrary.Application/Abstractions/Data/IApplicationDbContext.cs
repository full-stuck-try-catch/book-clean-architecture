using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Loans;
using BookLibrary.Domain.Reviews;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }

    DbSet<Library> Libraries { get; }

    DbSet<Loan> Loans { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Review> Reviews { get; }
}
