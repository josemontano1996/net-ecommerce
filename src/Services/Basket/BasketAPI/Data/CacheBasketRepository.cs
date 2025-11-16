namespace BasketAPI.Data;

public class CacheBasketRepository(IBasketRepository repository) : IBasketRepository
{
  public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellation = default)
  {
    return await repository.GetBasket(userName, cancellation);
  }

  public async Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
  {
    return await repository.StoreBasket(shoppingCart, cancellationToken);
  }

  public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
  {
    return await repository.DeleteBasket(userName, cancellationToken);
  }
}
