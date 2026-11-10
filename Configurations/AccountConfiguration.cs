using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalBank.Models;

namespace NationalBank.Configurations;

public class AccountConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", table =>
        {
            table.HasCheckConstraint(
                "CK_Accounts_Type",
                "[AccountType] IN ('Savings', 'Current', 'Business')"
            );
        });

        builder.HasKey(x => x.AccountNumber);

        builder.Property(x => x.AccountNumber)
            .HasMaxLength(30)
            .ValueGeneratedNever();

        builder.Property(x => x.CurrentBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.AccountType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.OpeningDate)
            .IsRequired();

        builder.Property(x => x.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(x => x.Branch)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.BranchCode)
            .OnDelete(DeleteBehavior.NoAction);
    }
}