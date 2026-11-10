using NationalBank.Models;

namespace NationalBank.Data;

public static class DbSeeder
{
    public static void Seed(BankContext db)
    {
        EnsureBranch(
            "CAI-01",
            "Cairo Main Branch",
            "Downtown, Cairo",
            "0223456789",
            "Omar Hassan",
            "omar.manager@example.com",
            "01012345678"
        );

        EnsureBranch(
            "ALX-01",
            "Alexandria Branch",
            "City Center, Alexandria",
            "034567890",
            "Mona Adel",
            "mona.manager@example.com",
            "01112345678"
        );

        db.SaveChanges();

        Console.WriteLine(
            "Branch/Manager seed data is ready."
        );

        void EnsureBranch(
            string code,
            string name,
            string address,
            string phone,
            string managerName,
            string managerEmail,
            string managerPhone)
        {
            var branch = db.Branches.Find(code);

            if (branch is null)
            {
                branch = new Branch
                {
                    Code = code,
                    Name = name,
                    Address = address,
                    PhoneNumber = phone
                };

                db.Branches.Add(branch);
            }

            bool managerExists = db.Managers.Any(
                x => x.BranchCode == code
            );

            if (!managerExists)
            {
                db.Managers.Add(new Manager
                {
                    FullName = managerName,
                    Email = managerEmail,
                    PhoneNumber = managerPhone,
                    HireDate = new DateTime(2024, 1, 1),
                    BranchCode = code,
                    Branch = branch
                });
            }
        }
    }
}