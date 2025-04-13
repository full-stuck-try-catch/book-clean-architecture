using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(200)
            .HasConversion(firstName => firstName.Value, value => new FirstName(value));

        builder.Property(user => user.LastName)
            .HasMaxLength(200)
            .HasConversion(firstName => firstName.Value, value => new LastName(value));

        builder.Property(user => user.Email)
            .HasMaxLength(400)
            .HasConversion(email => email.Value, value => new Email(value));

        builder.Property(user => user.PasswordHash)
            .HasConversion(passwordHash => passwordHash.Value, value => new PasswordHash(value));

        builder.Property(u => u.RegisteredAt).IsRequired();
        builder.Property(u => u.UpdatedAt);

        builder.HasMany(u => u.Loans)
            .WithOne()
            .HasForeignKey(l => l.UserId);

        builder.HasIndex(user => user.Email).IsUnique();
    }
}
