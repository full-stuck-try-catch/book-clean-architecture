using BookLibrary.Domain.Libraries;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.Infrastructure.Configurations;

internal sealed class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("libraries");

        builder.HasKey(library => library.Id);

        builder.Property(library => library.Name)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new LibraryName(value))
            .IsRequired();

        builder.HasMany(library => library.Books)
            .WithOne()
            .HasForeignKey(library => library.LibraryId);
    }
}
