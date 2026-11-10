using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalBank.Models;

namespace NationalBank.Configurations;

public class CustomerAccountConfiguration
    : IEntityTypeConfiguration<CustomerAccount>
{
    public void Configure(EntityTypeBuilder<CustomerAccount> builder)
    {
        builder.ToTable("CustomerAccounts", table =>
        {
            table.HasCheckConstraint(
                "CK_CustomerAccounts_OwnershipType",
                "[OwnershipType] IN ('Primary', 'CoHolder')"
            );

            table.HasCheckConstraint(
                "CK_CustomerAccounts_Status",
                "[AccountStatus] IN ('Active', 'Closed')"
            );
        });

        builder.HasKey(x => new
        {
            x.CustomerId,
            x.AccountNumber
        });

        builder.Property(x => x.AccountNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.OwnershipStartDate)
            .IsRequired();

        builder.Property(x => x.OwnershipType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AccountStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.CustomerAccounts)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.CustomerAccounts)
            .HasForeignKey(x => x.AccountNumber)
            .OnDelete(DeleteBehavior.NoAction);
    }
}