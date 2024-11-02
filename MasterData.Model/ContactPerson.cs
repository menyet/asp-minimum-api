namespace MasterData.Model;

public class ContactPerson
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Phone { get; set; }

    public required string Mail { get; set; }

    public Vendor Vendor { get; set; } = null!;

    public int VendorId { get; set; }
}