namespace NationalBank.Models;

public class CustomerAccount
{
    public int CustomerId { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public OwnershipType OwnershipType { get; set; }

    public DateTime OwnershipStartDate { get; set; }

    public AccountStatus AccountStatus { get; set; }

    public Customer Customer { get; set; } = null!;

    public Account Account { get; set; } = null!;
}