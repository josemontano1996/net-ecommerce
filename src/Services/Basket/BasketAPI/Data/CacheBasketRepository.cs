using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BasketAPI.Data;

public class CacheBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
  public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellation = default)
  {
    var cachedBasket = await cache.GetStringAsync(userName, cancellation);
    if (!string.IsNullOrEmpty(cachedBasket))
    {
      return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
    }

    var basket = await repository.GetBasket(userName, cancellation);
    await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    }, cancellation);

    return basket;
  }

  public async Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
  {
    await repository.StoreBasket(shoppingCart, cancellationToken);

    await cache.SetStringAsync(shoppingCart.UserName, JsonSerializer.Serialize(shoppingCart), new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    }, cancellationToken);

    return shoppingCart;
  }

  public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
  {
    await repository.DeleteBasket(userName, cancellationToken);

    await cache.RemoveAsync(userName, cancellationToken);

    return true;
  }
}
