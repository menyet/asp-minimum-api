namespace MasterData.Model;

public class Vendor
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Name2 { get; set; }

    public required string Address1 { get; set; }

    public required string Address2 { get; set; }

    public required string ZIP { get; set; }

    public required string Country { get; set; }

    public required string City { get; set; }

    public required string Mail { get; set; }

    public required string Phone { get; set; }

    public required string Notes { get; set; }

    public ICollection<BankAccount> BankAccount { get; set; } = null!;

    public ICollection<ContactPerson> ContactPersons { get; set; } = null!;
}
