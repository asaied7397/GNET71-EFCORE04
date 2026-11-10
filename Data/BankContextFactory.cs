using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NationalBank.Data;

public class BankContextFactory
    : IDesignTimeDbContextFactory<BankContext>
{
    public BankContext CreateDbContext(string[] args)
    {
        string? connection =
            Environment.GetEnvironmentVariable(
                "BANK_CONNECTION_STRING"
            );

        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException(
                "Set BANK_CONNECTION_STRING in this terminal first."
            );
        }

        var options = new DbContextOptionsBuilder<BankContext>()
            .UseSqlServer(connection)
            .Options;

        return new BankContext(options);
    }
}