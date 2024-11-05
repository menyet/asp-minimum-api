using System.Threading;

using MasterData.Model;

using Microsoft.AspNetCore.Authentication.Cookies;

namespace MasterData.Domain.Cache
{
    public interface IItemCache
    {
        public Task<T?> Get<T>(string key, CancellationToken cancellationToken) where T : class;

        public Task Set<T>(string key, T item, CancellationToken cancellationToken) where T : class;

        public Task Remove(string key, CancellationToken cancellationToken);
    }
}
