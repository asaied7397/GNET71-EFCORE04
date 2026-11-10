using Microsoft.EntityFrameworkCore;
using NationalBank.ConsoleUI;
using NationalBank.Data;

namespace NationalBank;

public static class Program
{
    public static int Main()
    {
        string? connection =
            Environment.GetEnvironmentVariable(
                "BANK_CONNECTION_STRING"
            );

        if (string.IsNullOrWhiteSpace(connection))
        {
            Console.Error.WriteLine(
                "Set BANK_CONNECTION_STRING in this terminal before running."
            );

            return 1;
        }

        var options = new DbContextOptionsBuilder<BankContext>()
            .UseSqlServer(connection)
            .Options;

        try
        {
            using (var db = new BankContext(options))
            {
                if (!db.Database.GetMigrations().Any())
                {
                    Console.Error.WriteLine(
                        "No migrations found. Run: " +
                        "dotnet ef migrations add InitialCreate"
                    );

                    return 1;
                }

                db.Database.Migrate();

                DbSeeder.Seed(db);
            }

            new BankMenu(options).Run();

            Console.WriteLine(
                "Goodbye!"
            );

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(
                "Input closed. Goodbye!"
            );

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"Application failed: {ex.GetBaseException().Message}"
            );

            return 1;
        }
    }
}