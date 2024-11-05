using MasterData.Domain;
using MasterData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MasterData.Domain.Exceptions;

namespace MasterData.Host.Endpoints
{
    public static class ContactPersonOperations
    {
        public static IEndpointRouteBuilder ConfigureContactPersonOperations(this IEndpointRouteBuilder builder)
        {
            var subGroup = builder.MapGroup("{vendorId}/contact-persons");

            subGroup.MapGet("/", GetContactPersons).ConfigureEndpoint("Get contact persons of a vendor", "Contact persons");
            subGroup.MapGet("/{contactPersonId}", GetContactPerson).ConfigureEndpoint("Get details of a contact person", "Contact persons");
            subGroup.MapPost("/", AddContactPerson).ConfigureEndpoint("Add contact person", "Contact persons");
            subGroup.MapPut("/{contactPersonId}", UpdateContactPerson).ConfigureEndpoint("Update contact person", "Contact persons");

            return builder;
        }

        public static async Task<int> AddContactPerson(int vendorId, NewContactPersonModel payload, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var contactPerson = new ContactPerson
            {
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                Phone = payload.Phone,
                Mail = payload.Mail,
                VendorId = vendorId
            };

            facade.Add(contactPerson);

            await facade.Save(cancellationToken);

            return contactPerson.Id;
        }

        public static async Task UpdateContactPerson(int vendorId, int contactPersonId, UpdateContactPersonModel payload, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var contactPerson = await facade.Get<ContactPerson>(contactPersonId, cancellationToken)
                ?? throw new NotFoundException<ContactPerson>();

            if (vendorId != contactPerson.VendorId)
            {
                throw new NotFoundException<ContactPerson>();
            }

            contactPerson.FirstName = payload.FirstName;
            contactPerson.LastName = payload.LastName;
            contactPerson.Phone = payload.Phone;
            contactPerson.Mail = payload.Mail;

            await facade.Save(cancellationToken);
        }

        public static async Task<ContactPersonModel[]> GetContactPersons(int vendorId, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var contactPersons = await facade.GetContactPersons(vendorId, cancellationToken);

            return contactPersons;
        }

        public static async Task<ContactPersonDetails?> GetContactPerson(int vendorId, int contactPersonId, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var contactPerson = await facade.Get<ContactPerson>(contactPersonId, cancellationToken)
                ?? throw new NotFoundException<ContactPerson>();

            if (vendorId != contactPerson.VendorId)
            {
                throw new NotFoundException<ContactPerson>();
            }

            return new ContactPersonDetails
            { 
                Id = contactPerson.Id,
                FirstName = contactPerson.FirstName,
                LastName = contactPerson.LastName,
                Phone = contactPerson.Phone,
                Mail = contactPerson.Mail
            };
        }

        public interface IFacade : IBasicOperations
        {
            public Task<ContactPersonModel[]> GetContactPersons(int vendorId, CancellationToken cancellationToken) => 
                Db.Set<ContactPerson>()
                    .Where(contactPerson => contactPerson.VendorId == vendorId)
                    .Select(contactPerson => new ContactPersonModel(contactPerson.Id, contactPerson.FirstName, contactPerson.LastName)).ToArrayAsync(cancellationToken);
        }

        public record Facade(DatabaseContext Db) : IFacade;

        public record ContactPersonModel(int Id, string FirstName, string LastName);

        public class ContactPersonDetails
        {
            public required int Id { get; set; }

            public required string FirstName { get; set; }

            public required string LastName { get; set; }

            public required string Phone { get; set; }

            public required string Mail { get; set; }
        }

        public class NewContactPersonModel
        {
            public required string FirstName { get; set; }

            public required string LastName { get; set; }

            public required string Phone { get; set; }

            public required string Mail { get; set; }
        }

        public class UpdateContactPersonModel
        {
            public required string FirstName { get; set; }

            public required string LastName { get; set; }

            public required string Phone { get; set; }

            public required string Mail { get; set; }
        }
    }
}
