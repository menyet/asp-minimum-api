namespace MasterData.Model;

using System.ComponentModel.DataAnnotations;

public class BankAccount
{
    public int Id { get; set; }

    public required string IBAN { get; set; }

    public required string BIC { get; set; }

    public required string Name { get; set; }

    public Vendor Vendor { get; set; } = null!;

    public int VendorId { get; set; }
}
