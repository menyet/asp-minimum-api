using MasterData.Domain;
using MasterData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MasterData.Domain.Exceptions;

namespace MasterData.Host.Endpoints
{
    public static class BankAccountOperations
    {
        public static IEndpointRouteBuilder ConfigureBankAccountOperations(this IEndpointRouteBuilder builder)
        {
            var subGroup = builder.MapGroup("{vendorId}/bank-accounts");

            subGroup.MapGet("/", GetBankAccounts).ConfigureEndpoint("Get bank accounts of a vendor", "Bank accounts");
            subGroup.MapGet("/{bankAccountId}", GetBankAccount).ConfigureEndpoint("Get details of a vendor bank account", "Bank accounts");
            subGroup.MapPost("/", AddBankAccount).ConfigureEndpoint("Add bank account", "Bank accounts");
            subGroup.MapPut("/{bankAccountId}", UpdateBankAccount).ConfigureEndpoint("Update bank accounts", "Bank accounts");

            return builder;
        }

        public static async Task<int> AddBankAccount(int vendorId, NewBankAccountModel payload, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var bankAccount = new BankAccount
            { 
                BIC = payload.BIC,
                IBAN = payload.IBAN,
                Name = payload.Name,
                VendorId = vendorId
            };

            facade.Add(bankAccount);

            await facade.Save(cancellationToken);

            return bankAccount.Id;
        }

        public static async Task UpdateBankAccount(int vendorId, int bankAccountId, UpdateBankAccountModel payload, [FromServices] IFacade facade, [FromServices] IDistributedCache cache, CancellationToken cancellationToken)
        {
            var bankAccount = await facade.Get<BankAccount>(bankAccountId, cancellationToken)
                ?? throw new NotFoundException<BankAccount>();

            if (vendorId != bankAccount.VendorId)
            {
                throw new NotFoundException<BankAccount>();
            }

            bankAccount.Name = payload.Name;
            bankAccount.IBAN = payload.IBAN;
            bankAccount.BIC = payload.BIC;

            await facade.Save(cancellationToken);

            await cache.RemoveAsync("BankAccounts", cancellationToken);
            await cache.RemoveAsync($"BankAccounts-{bankAccountId}", cancellationToken);
        }

        public static async Task<BankAccountModel[]> GetBankAccounts(int vendorId, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var bankAccounts = await facade.GetBankAccounts(vendorId, cancellationToken);

            return bankAccounts;
        }

        public static async Task<BankAccountDetails?> GetBankAccount(int vendorId, int bankAccountId, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var bankAccount = await facade.Get<BankAccount>(bankAccountId, cancellationToken)
                ?? throw new NotFoundException<BankAccount>();

            if (vendorId != bankAccount.VendorId)
            {
                throw new NotFoundException<BankAccount>();
            }

            return new BankAccountDetails
            {
                Id = bankAccount.Id,
                IBAN = bankAccount.IBAN,
                BIC = bankAccount.BIC,
                Name = bankAccount.Name
            };
        }

        public interface IFacade : IBasicDatabaseOperations
        {
            public Task<BankAccountModel[]> GetBankAccounts(int vendorId, CancellationToken cancellationToken) => 
                Db.Set<BankAccount>()
                    .Where(bankAccount => bankAccount.VendorId == vendorId)
                    .Select(bankAccount => new BankAccountModel(bankAccount.Id, bankAccount.Name, bankAccount.IBAN, bankAccount.BIC)).ToArrayAsync(cancellationToken);
        }

        public record Facade(DatabaseContext Db) : IFacade;

        public record BankAccountModel(int Id, string Name, string IBAN, string BIC);

        public class BankAccountDetails
        {
            public required int Id { get; set; }

            public required string IBAN { get; set; }

            public required string BIC { get; set; }

            public required string Name { get; set; }
        }

        public class NewBankAccountModel
        {
            public required string IBAN { get; set; }

            public required string BIC { get; set; }

            public required string Name { get; set; }
        }

        public class UpdateBankAccountModel
        {
            public required string IBAN { get; set; }

            public required string BIC { get; set; }

            public required string Name { get; set; }
        }
    }
}
