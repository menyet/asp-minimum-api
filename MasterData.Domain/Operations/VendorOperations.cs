using MasterData.Domain;
using MasterData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MasterData.Domain.Exceptions;
using MasterData.Domain.Cache;

namespace MasterData.Host.Endpoints
{
    public static class VendorOperations
    {
        public const string CacheKey = "vendors";

        public static IEndpointRouteBuilder ConfigureVendorOperations(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", GetVendors).ConfigureEndpoint("Get all vendors", "Vendors");
            builder.MapGet("/{vendorId}", GetVendor).ConfigureEndpoint("Get details of a vendor", "Vendors");
            builder.MapPost("/", AddVendor).ConfigureEndpoint("Add vendor", "Vendors");
            builder.MapPut("/{vendorId}", UpdateVendor).ConfigureEndpoint("Update vendor", "Vendors");

            return builder;
        }

        public static async Task<int> AddVendor(NewVendorModel payload, [FromServices] IFacade facade, [FromServices] IItemCache cache, CancellationToken cancellationToken)
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

            await cache.Remove(CacheKey, cancellationToken);

            return vendor.Id;
        }

        public static async Task UpdateVendor(int vendorId, NewVendorModel payload, [FromServices] IFacade facade, [FromServices] IItemCache cache, CancellationToken cancellationToken)
        {
            var vendor = await facade.Get<Vendor>(vendorId, cancellationToken)
                ?? throw new NotFoundException<Vendor>();

            vendor.Name = payload.Name;
            vendor.Name2 = payload.Name2;
            vendor.Address1 = payload.Address1;
            vendor.Address2 = payload.Address2;
            vendor.ZIP = payload.ZIP;
            vendor.Country = payload.Country;
            vendor.City = payload.City;
            vendor.Mail = payload.Mail;
            vendor.Phone = payload.Phone;
            vendor.Notes = payload.Notes;

            await facade.Save(cancellationToken);

            await cache.Remove(CacheKey, cancellationToken);
            await cache.Remove($"{CacheKey}-{vendorId}", cancellationToken);
        }

        public static async Task<VendorModel[]> GetVendors([FromServices] IFacade facade, [FromServices] IItemCache cache, CancellationToken cancellationToken)
        {
            var vendors = await cache.Get<VendorModel[]>(CacheKey, cancellationToken);

            if (vendors is null)
            {
                vendors = await facade.GetVendors(cancellationToken);

                await cache.Set(CacheKey, vendors, cancellationToken);
            }

            return vendors;
        }

        public static async Task<VendorDetails?> GetVendor(int vendorId, [FromServices] IFacade facade, [FromServices] IItemCache cache, CancellationToken cancellationToken)
        {
            var vendorModel = await cache.Get<VendorDetails>($"{CacheKey}-{vendorId}", cancellationToken);

            if (vendorModel is null)
            {
                var vendor = await facade.Get<Vendor>(vendorId, cancellationToken)
                    ?? throw new NotFoundException<Vendor>();

                vendorModel = new VendorDetails(vendor.Id, vendor.Name, vendor.Name2, vendor.Address1, vendor.Address2);
            }

            return vendorModel;
        }


        public interface IFacade : IBasicDatabaseOperations
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
