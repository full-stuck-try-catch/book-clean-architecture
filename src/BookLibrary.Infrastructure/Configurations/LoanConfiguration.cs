using BookLibrary.Domain.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookLibrary.Domain.Users;

namespace BookLibrary.Infrastructure.Configurations;

internal sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");

        builder.HasKey(loan => loan.Id);

        builder.Property(loan => loan.UserId)
            .IsRequired();

        builder.OwnsOne(loan => loan.Period, periodBuilder =>
        {
            periodBuilder.Property(p => p.StartDate).IsRequired();
            periodBuilder.Property(p => p.EndDate).IsRequired();
        });

        builder.Property(loan => loan.ReturnedAt);

        // Indexes for performance
        builder.HasIndex(loan => loan.UserId);
        builder.HasIndex(loan => loan.BookId);
    }
}
