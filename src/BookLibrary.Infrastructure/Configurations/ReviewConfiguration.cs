using BookLibrary.Domain.Reviews;
using BookLibrary.Domain.Books;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(review => review.Id);

        builder.Property(review => review.BookId)
            .IsRequired();

        builder.Property(review => review.UserId)
            .IsRequired();

        builder.Property(review => review.Comment)
            .HasMaxLength(1000)
            .HasConversion(comment => comment.Value, value => new Comment(value))
            .IsRequired();

        builder.Property(review => review.Rating)
            .HasConversion(
                rating => rating!.Value,
                value => Rating.Create(value).Value)
            .IsRequired(false);

        builder.Property(review => review.CreatedAt)
            .IsRequired();

        builder.Property(review => review.UpdatedAt);

        // Configure foreign key relationships
        builder.HasOne<Book>()
            .WithMany()
            .HasForeignKey(review => review.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(review => review.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(review => review.BookId);
        builder.HasIndex(review => review.UserId);
        builder.HasIndex(review => review.CreatedAt);

        // Ensure one review per user per book
        builder.HasIndex(review => new { review.BookId, review.UserId })
            .IsUnique();
    }
} 