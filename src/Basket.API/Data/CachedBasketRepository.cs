using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class CachedBasketRepository(
        BasketRepository repository,
        IDistributedCache cache) : IBasketRepository
    {
        private static readonly DistributedCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        public async Task<ShoppingCart> GetBasket(
            string userName,
            CancellationToken cancellationToken = default)
        {
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);

            if (!string.IsNullOrWhiteSpace(cachedBasket))
            {
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
            }

            var basket = await repository.GetBasket(userName, cancellationToken);
            await cache.SetStringAsync(
                userName,
                JsonSerializer.Serialize(basket),
                CacheOptions,
                cancellationToken);

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(
            ShoppingCart basket,
            CancellationToken cancellationToken = default)
        {
            var storedBasket = await repository.StoreBasket(basket, cancellationToken);

            await cache.SetStringAsync(
                storedBasket.UserName,
                JsonSerializer.Serialize(storedBasket),
                CacheOptions,
                cancellationToken);

            return storedBasket;
        }

        public async Task<bool> DeleteBasket(
            string userName,
            CancellationToken cancellationToken = default)
        {
            var result = await repository.DeleteBasket(userName, cancellationToken);
            await cache.RemoveAsync(userName, cancellationToken);

            return result;
        }
    }
}
