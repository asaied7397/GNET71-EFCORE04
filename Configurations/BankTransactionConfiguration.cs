using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalBank.Models;

namespace NationalBank.Configurations;

public class BankTransactionConfiguration
    : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.ToTable("Transactions", table =>
        {
            table.HasCheckConstraint(
                "CK_Transactions_Amount",
                "[Amount] > 0"
            );

            table.HasCheckConstraint(
                "CK_Transactions_Type",
                "[TransactionType] IN ('Deposit', 'Withdrawal')"
            );
        });

        builder.HasKey(x => x.TransactionNumber);

        builder.Property(x => x.TransactionNumber)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TransactionDate)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.AccountNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AccountNumber)
            .OnDelete(DeleteBehavior.NoAction);
    }
}