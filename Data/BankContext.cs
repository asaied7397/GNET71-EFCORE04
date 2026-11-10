using Microsoft.EntityFrameworkCore;
using NationalBank.Models;

namespace NationalBank.Data;

public class BankContext(
    DbContextOptions<BankContext> options
) : DbContext(options)
{
    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Manager> Managers => Set<Manager>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<CustomerAccount> CustomerAccounts =>
        Set<CustomerAccount>();

    public DbSet<BankTransaction> Transactions =>
        Set<BankTransaction>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BankContext).Assembly
        );
    }
}