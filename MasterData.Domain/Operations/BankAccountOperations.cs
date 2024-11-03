using System.Text.Json;
using System.Text;

using MasterData.Domain;
using MasterData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace MasterData.Host.Endpoints
{
    public static class BankAccountOperations
    {
        public static IEndpointRouteBuilder ConfigureBankAccountOperations(this IEndpointRouteBuilder builder)
        {
            var subGroup = builder.MapGroup("{vendorId}/bank-accounts");

            subGroup.MapGet("/", GetBankAccounts).RequireAuthorization();
            subGroup.MapGet("/{bankAccountId}", GetBankAccount).RequireAuthorization(builder => builder.RequireRole("MasterDataManager"));
            subGroup.MapPost("/", AddBankAccount).RequireAuthorization();

            return builder;
        }

        public static async Task<int> AddBankAccount(int vendorId, NewBankAccountModel payload, [FromServices] IFacade facade, [FromServices] IDistributedCache cache, CancellationToken cancellationToken)
        {
            var bankAccount = new BankAccount
            { 
                BIC = payload.BIC,
                IBAN = payload.IBAN,
                Name = payload.Name,
                VendorId = vendorId //payload.VendorId
            };

            facade.Add(bankAccount);

            await facade.Save(cancellationToken);

            await cache.RemoveAsync("BankAccounts", cancellationToken);

            return bankAccount.Id;
        }

        public static async Task<BankAccountModel[]> GetBankAccounts(int vendorId, [FromServices] IFacade facade, [FromServices] IDistributedCache cache, CancellationToken cancellationToken)
        {
            var cachedBankAccounts = await cache.GetAsync("BankAccounts");

            if (cachedBankAccounts is null)
            {
                var bankAccounts = await facade.GetBankAccounts(vendorId, cancellationToken);

                await cache.SetAsync("BankAccounts", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bankAccounts)), new()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(10)
                });

                return bankAccounts;
            }

            // TODO: handle null
            return JsonSerializer.Deserialize<BankAccountModel[]>(cachedBankAccounts) ?? [];
        }

        public static async Task<BankAccountDetails?> GetBankAccount(int vendorId, int bankAccountId, [FromServices] IFacade facade, CancellationToken cancellationToken)
        {
            var bankAccount = await facade.Get<BankAccount>(bankAccountId, cancellationToken)
                ?? throw new InvalidOperationException();

            if (vendorId != bankAccount.VendorId)
            {
                throw new InvalidOperationException();
            }

            return new BankAccountDetails
            {
                Id = bankAccount.Id,
                IBAN = bankAccount.IBAN,
                BIC = bankAccount.BIC,
                Name = bankAccount.Name
            };
        }

        public interface IFacade : IBasicOperations
        {
            public Task<BankAccountModel[]> GetBankAccounts(int vendorId, CancellationToken cancellationToken) => 
                Db.Set<BankAccount>()
                    .Where(BankAccount => BankAccount.VendorId == vendorId)
                    .Select(BankAccount => new BankAccountModel(BankAccount.Id, BankAccount.Name, BankAccount.IBAN, BankAccount.BIC)).ToArrayAsync(cancellationToken);
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

            public int VendorId { get; set; }
        }
    }
}
