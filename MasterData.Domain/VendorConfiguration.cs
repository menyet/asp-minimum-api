using MasterData.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Domain
{
    internal class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("Vendors");
            builder.HasMany(vendor => vendor.ContactPersons).WithOne(person => person.Vendor).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(vendor => vendor.BankAccounts).WithOne(account => account.Vendor).OnDelete(DeleteBehavior.Restrict);
        }
    }
}