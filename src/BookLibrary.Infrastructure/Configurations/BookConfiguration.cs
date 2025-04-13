using BookLibrary.Domain.Books;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.Infrastructure.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .HasMaxLength(500)
            .HasConversion(title => title.Value, value => new BookTitle(value));

        builder.OwnsOne(book => book.Author, authorBuilder =>
        {
            authorBuilder.Property(a => a.AuthorFirstName).HasMaxLength(200).IsRequired();
            authorBuilder.Property(a => a.AuthorLastName).HasMaxLength(200).IsRequired();
            authorBuilder.Property(a => a.AuthorCountry).HasMaxLength(200).IsRequired();
        });

        builder.Property(book => book.Quantity)
            .IsRequired();

        builder.Property(book => book.AvailableQuantity)
            .IsRequired();

        builder.Property(book => book.Status)
            .HasConversion(
                status => (int)status,
                value => (BookStatus)value)
            .IsRequired();
    }
}
