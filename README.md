# National Bank Management System

EF Core Code-First console application using .NET 10 and SQL Server.

## Features

- Branch–Manager one-to-one relationship.
- Branch–Account and Account–Transaction relationships.
- Customer–Account many-to-many ownership through CustomerAccount.
- Composite ownership key.
- Account status stored on the ownership relationship.
- Repeat-safe Branch and Manager seeding.
- Interactive menu with input validation.
- Eager loading using Include and ThenInclude.
- Ownership deletion with last-owner account cleanup.

## Setup

Set `BANK_CONNECTION_STRING` in the terminal, then run:

```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Create `InitialCreate` only once. Keep the generated migrations in Git.

## Menu

1. Add customer.
2. Open account or link an existing account to another customer.
3. Toggle ownership account status.
4. Remove ownership.
5. List customers with their accounts.
6. Exit.

## Notes

- Initial branch codes: CAI-01 and ALX-01.
- New accounts start with a zero balance.
- Last-owner account deletion is refused when balance or transaction history exists.
- Text lengths, national-ID uniqueness and transaction-type values are implementation choices where the supplied assignment is silent.
- Exclude bin/obj and never commit passwords.
