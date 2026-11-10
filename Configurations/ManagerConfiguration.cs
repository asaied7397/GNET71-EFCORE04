using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalBank.Models;

namespace NationalBank.Configurations;

public class ManagerConfiguration
    : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.ToTable("Managers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.HireDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(x => x.Branch)
            .WithOne(x => x.Manager)
            .HasForeignKey<Manager>(x => x.BranchCode)
            .OnDelete(DeleteBehavior.NoAction);
    }
}