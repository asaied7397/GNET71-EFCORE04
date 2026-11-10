namespace NationalBank.Models;

public class Account
{
    public string AccountNumber { get; set; } = string.Empty;

    public decimal CurrentBalance { get; set; }

    public AccountType AccountType { get; set; }

    public DateTime OpeningDate { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public Branch Branch { get; set; } = null!;

    public ICollection<CustomerAccount> CustomerAccounts { get; set; }
        = new List<CustomerAccount>();

    public ICollection<BankTransaction> Transactions { get; set; }
        = new List<BankTransaction>();
}