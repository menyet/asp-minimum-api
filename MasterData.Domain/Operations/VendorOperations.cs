using MasterData.Domain;
using MasterData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Host.Endpoints
{
    public static class VendorOperations
    {
        public static void ConfigureVendorOperations(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", GetVendors).RequireAuthorization();
            builder.MapGet("/{vendorId}", GetVendor).RequireAuthorization();
            builder.MapPost("/", AddVendor).RequireAuthorization();
        }

        public static async Task<int> AddVendor(NewVendorModel payload, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var vendor = new Vendor
            {
                Name = payload.Name,
                Name2 = payload.Name2,
                Address1 = payload.Address1,
                Address2 = payload.Address2,
                ZIP = payload.ZIP,
                Country = payload.Country,
                City = payload.City,
                Mail = payload.Mail,
                Phone = payload.Phone,
                Notes = payload.Notes
            };

            facade.Add(vendor);

            await facade.Save(cancellationToken);

            return vendor.Id;
        }

        public static Task<VendorModel[]> GetVendors([FromServices] IFacade facade, CancellationToken cancellationToken) => facade.GetVendors(cancellationToken);

        public static async Task<VendorDetails?> GetVendor(int id, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var vendor = await facade.Get<Vendor>(id, cancellationToken)
                ?? throw new InvalidOperationException();

            return new VendorDetails(vendor.Id, vendor.Name, vendor.Name2, vendor.Address1, vendor.Address2);
        }


        public interface IFacade : IBasicOperations
        {
            public Task<VendorModel[]> GetVendors(CancellationToken cancellationToken) => Db.Set<Vendor>().Select(vendor => new VendorModel(vendor.Id, vendor.Name, vendor.Name2)).ToArrayAsync(cancellationToken);
        }

        public record Facade(DatabaseContext Db) : IFacade;

        public record VendorModel(int Id, string Name, string Name2);

        public record VendorDetails(int Id, string Name, string Name2, string Address1, string Address2);

        public class NewVendorModel
        {
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
        }
    }
}
