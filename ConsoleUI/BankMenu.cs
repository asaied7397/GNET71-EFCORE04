using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NationalBank.Data;
using NationalBank.Models;
using NationalBank.Services;

namespace NationalBank.ConsoleUI;

public class BankMenu(
    DbContextOptions<BankContext> options)
{
    public void Run()
    {
        while (true)
        {
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }

            Console.WriteLine(
                "==============================================="
            );

            Console.WriteLine(
                "       National Bank - Management"
            );

            Console.WriteLine(
                "==============================================="
            );

            Console.WriteLine(
                "1) Add a new Customer"
            );

            Console.WriteLine(
                "2) Open a new Account for a Customer"
            );

            Console.WriteLine(
                "3) Toggle Account Status (Active / Closed)"
            );

            Console.WriteLine(
                "4) Remove an Account from a Customer"
            );

            Console.WriteLine(
                "5) List all Customers (with accounts)"
            );

            Console.WriteLine(
                "0) Exit"
            );

            int choice = ConsoleInput.ReadInteger(
                "Enter choice: ",
                0,
                5
            );

            if (choice == 0)
            {
                return;
            }

            try
            {
                // A fresh context for each menu operation.
                using var db = new BankContext(options);

                var service = new BankService(db);

                switch (choice)
                {
                    case 1:
                        AddCustomer(service);
                        break;

                    case 2:
                        OpenAccount(service);
                        break;

                    case 3:
                        ToggleStatus(service);
                        break;

                    case 4:
                        RemoveOwnership(service);
                        break;

                    case 5:
                        ListCustomers(service);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is SqlException sql &&
                    sql.Number is 2601 or 2627)
                {
                    Console.WriteLine(
                        "Duplicate record: this identifier " +
                        "or ownership link already exists."
                    );
                }
                else if (
                    ex.GetBaseException() is SqlException fk &&
                    fk.Number == 547)
                {
                    Console.WriteLine(
                        "Operation refused: related records " +
                        "or database constraints prevent this change."
                    );
                }
                else
                {
                    Console.WriteLine(
                        "Could not save the operation. " +
                        "No partial changes were committed. Please retry."
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Operation could not be completed: {ex.Message}"
                );
            }

            ConsoleInput.Pause();
        }
    }

    private static void AddCustomer(BankService service)
    {
        Console.WriteLine(
            "\n--- Add New Customer ---"
        );

        string name = ConsoleInput.ReadRequired(
            "Full name: ",
            150
        );

        string nationalId = ConsoleInput.ReadNationalId();

        DateTime birth = ConsoleInput.ReadDateOfBirth();

        string email = ConsoleInput.ReadEmail();

        string phone = ConsoleInput.ReadPhone();

        string address = ConsoleInput.ReadRequired(
            "Address: ",
            300
        );

        CustomerType type =
            ConsoleInput.ReadEnum<CustomerType>(
                "Customer type:"
            );

        OperationResult result = service.AddCustomer(
            name,
            nationalId,
            birth,
            email,
            phone,
            address,
            type
        );

        Console.WriteLine(result.Message);
    }

    private static void OpenAccount(BankService service)
    {
        Console.WriteLine(
            "\n--- Open New Account / Link Joint Owner ---"
        );

        string number = ConsoleInput.ReadCode(
            "Account number: ",
            30
        );

        AccountType type =
            ConsoleInput.ReadEnum<AccountType>(
                "Account type:"
            );

        string branch = ConsoleInput.ReadCode(
            "Branch code: ",
            20
        );

        int customerId = ConsoleInput.ReadInteger(
            "Customer Id: ",
            1
        );

        OwnershipType ownership =
            ConsoleInput.ReadEnum<OwnershipType>(
                "Ownership role:"
            );

        OperationResult result = service.OpenAccount(
            number,
            type,
            branch,
            customerId,
            ownership
        );

        Console.WriteLine(result.Message);
    }

    private static void ToggleStatus(BankService service)
    {
        Console.WriteLine(
            "\n--- Update Account Status ---"
        );

        string number = ConsoleInput.ReadCode(
            "Account number: ",
            30
        );

        int customerId = ConsoleInput.ReadInteger(
            "Customer Id: ",
            1
        );

        OperationResult result =
            service.ToggleAccountStatus(
                number,
                customerId
            );

        Console.WriteLine(result.Message);
    }

    private static void RemoveOwnership(BankService service)
    {
        Console.WriteLine(
            "\n--- Remove Account From Customer ---"
        );

        string number = ConsoleInput.ReadCode(
            "Account number: ",
            30
        );

        int customerId = ConsoleInput.ReadInteger(
            "Customer Id: ",
            1
        );

        OperationResult result =
            service.RemoveAccountFromCustomer(
                number,
                customerId
            );

        Console.WriteLine(result.Message);
    }

    private static void ListCustomers(BankService service)
    {
        Console.WriteLine(
            "\n--- All Customers ---"
        );

        var customers = service.GetCustomersWithAccounts();

        if (customers.Count == 0)
        {
            Console.WriteLine(
                "No customers found."
            );

            return;
        }

        foreach (var customer in customers)
        {
            Console.WriteLine(
                $"\n#{customer.Id} {customer.FullName} " +
                $"({customer.CustomerType})"
            );

            if (customer.CustomerAccounts.Count == 0)
            {
                Console.WriteLine(
                    "  (no accounts)"
                );

                continue;
            }

            foreach (var link in customer.CustomerAccounts
                         .OrderBy(x => x.AccountNumber))
            {
                var account = link.Account;

                Console.WriteLine(
                    $"  {account.AccountNumber,-20} " +
                    $"{account.AccountType,-10} " +
                    $"Balance: {account.CurrentBalance,14:N2}  " +
                    $"{link.OwnershipType,-8} " +
                    $"{link.AccountStatus,-6} " +
                    $"@ {account.Branch.Name}"
                );
            }
        }
    }
}