namespace NationalBank.Models;

public class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public CustomerType CustomerType { get; set; }

    public ICollection<CustomerAccount> CustomerAccounts { get; set; }
        = new List<CustomerAccount>();
}