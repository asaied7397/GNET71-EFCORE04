namespace NationalBank.Models;

public class BankTransaction
{
    public long TransactionNumber { get; set; }

    public DateTime TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public TransactionType TransactionType { get; set; }

    public string? Note { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public Account Account { get; set; } = null!;
}