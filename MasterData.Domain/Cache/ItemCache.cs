using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

namespace MasterData.Domain.Cache
{
    public class ItemCache(IDistributedCache cache) : IItemCache
    {
        private const int ExpirationInSeconds = 10;

        public async Task<T?> Get<T>(string key, CancellationToken cancellationToken) where T : class
        {
            var rawItem = await cache.GetAsync(key, cancellationToken);

            if (rawItem is null)
            {
                return null;
            }

            var item = JsonSerializer.Deserialize<T>(rawItem);

            return item;
        }

        public Task Remove(string key, CancellationToken cancellationToken) => cache.RemoveAsync(key, cancellationToken);

        public Task Set<T>(string key, T item, CancellationToken cancellationToken) where T : class => cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item)), new()
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(ExpirationInSeconds)
        });
    }
}
