using System.Data;
using Microsoft.EntityFrameworkCore;
using NationalBank.Data;
using NationalBank.Models;

namespace NationalBank.Services;

public sealed record OperationResult(
    bool Success,
    string Message
);

public class BankService(BankContext db)
{
    public OperationResult AddCustomer(
        string name,
        string nationalId,
        DateTime birthDate,
        string email,
        string phone,
        string address,
        CustomerType type)
    {
        bool exists = db.Customers.Any(
            x => x.NationalId == nationalId
        );

        if (exists)
        {
            return new OperationResult(
                false,
                "A customer with this national ID already exists."
            );
        }

        var customer = new Customer
        {
            FullName = name,
            NationalId = nationalId,
            DateOfBirth = birthDate,
            Email = email,
            PhoneNumber = phone,
            Address = address,
            CustomerType = type
        };

        db.Customers.Add(customer);

        db.SaveChanges();

        return new OperationResult(
            true,
            $"Customer created successfully. CustomerId = {customer.Id}"
        );
    }

    public OperationResult OpenAccount(
        string accountNumber,
        AccountType type,
        string branchCode,
        int customerId,
        OwnershipType ownership)
    {
        // The checks and insert belong to one transaction.
        using var transaction = db.Database.BeginTransaction(
            IsolationLevel.Serializable
        );

        bool branchExists = db.Branches.Any(
            x => x.Code == branchCode
        );

        if (!branchExists)
        {
            return new OperationResult(
                false,
                $"Branch '{branchCode}' does not exist."
            );
        }

        bool customerExists = db.Customers.Any(
            x => x.Id == customerId
        );

        if (!customerExists)
        {
            return new OperationResult(
                false,
                $"Customer #{customerId} does not exist."
            );
        }

        bool linkExists = db.CustomerAccounts.Any(
            x => x.CustomerId == customerId &&
                 x.AccountNumber == accountNumber
        );

        if (linkExists)
        {
            return new OperationResult(
                false,
                "This customer is already linked to this account."
            );
        }

        var account = db.Accounts.Find(accountNumber);

        bool isNew = account is null;

        if (account is not null &&
            (account.BranchCode != branchCode ||
             account.AccountType != type))
        {
            return new OperationResult(
                false,
                "Existing account has a different branch or account type. " +
                "No changes were saved."
            );
        }

        if (account is null)
        {
            account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = type,
                BranchCode = branchCode,
                CurrentBalance = 0m,
                OpeningDate = DateTime.UtcNow
            };

            db.Accounts.Add(account);
        }

        db.CustomerAccounts.Add(new CustomerAccount
        {
            CustomerId = customerId,
            AccountNumber = accountNumber,
            Account = account,
            OwnershipType = ownership,
            OwnershipStartDate = DateTime.UtcNow,
            AccountStatus = AccountStatus.Active
        });

        db.SaveChanges();

        transaction.Commit();

        string action = isNew
            ? "created and linked"
            : "linked";

        return new OperationResult(
            true,
            $"Account '{accountNumber}' {action} to customer " +
            $"{customerId} as {ownership} owner."
        );
    }

    public OperationResult ToggleAccountStatus(
        string accountNumber,
        int customerId)
    {
        // Status belongs to CustomerAccount, not Account.
        var link = db.CustomerAccounts.Find(
            customerId,
            accountNumber
        );

        if (link is null)
        {
            return new OperationResult(
                false,
                "No account ownership link matches this customer " +
                "and account number."
            );
        }

        link.AccountStatus =
            link.AccountStatus == AccountStatus.Active
                ? AccountStatus.Closed
                : AccountStatus.Active;

        db.SaveChanges();

        return new OperationResult(
            true,
            $"Status updated to {link.AccountStatus} " +
            $"for customer {customerId}."
        );
    }

    public OperationResult RemoveAccountFromCustomer(
        string accountNumber,
        int customerId)
    {
        // Keep the last-owner check and deletion in one transaction.
        using var transaction = db.Database.BeginTransaction(
            IsolationLevel.Serializable
        );

        var link = db.CustomerAccounts.Find(
            customerId,
            accountNumber
        );

        if (link is null)
        {
            return new OperationResult(
                false,
                "Ownership link not found."
            );
        }

        bool hasOtherOwners = db.CustomerAccounts.Any(
            x => x.AccountNumber == accountNumber &&
                 x.CustomerId != customerId
        );

        Account? account = null;

        if (!hasOtherOwners)
        {
            account = db.Accounts.Find(accountNumber);

            if (account is null)
            {
                return new OperationResult(
                    false,
                    "Account not found."
                );
            }

            // Additional safeguard:
            // do not delete an account containing funds or history.
            bool hasTransactions = db.Transactions.Any(
                x => x.AccountNumber == accountNumber
            );

            if (account.CurrentBalance != 0m || hasTransactions)
            {
                return new OperationResult(
                    false,
                    "Cannot remove the last owner: account has " +
                    "a balance or transaction history."
                );
            }
        }

        db.CustomerAccounts.Remove(link);

        if (account is not null)
        {
            db.Accounts.Remove(account);
        }

        db.SaveChanges();

        transaction.Commit();

        string message = hasOtherOwners
            ? "Ownership link deleted. " +
              "Account kept for the remaining owners."
            : "Ownership link deleted.\n" +
              $"That was the last owner - account '{accountNumber}' " +
              "was also removed.";

        return new OperationResult(
            true,
            message
        );
    }

    public List<Customer> GetCustomersWithAccounts()
    {
        return db.Customers
            .AsNoTracking()
            .Include(customer => customer.CustomerAccounts)
                .ThenInclude(link => link.Account)
                    .ThenInclude(account => account.Branch)
            .OrderBy(customer => customer.Id)
            .ToList();
    }
}