namespace BasketAPI.Data;

public class BasketRepository(IDocumentSession session) : IBasketRepository
{

  public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellation = default)
  {
    var basket = await session.LoadAsync<ShoppingCart>(userName, cancellation);
    return basket ?? throw new BasketNotFoundException(userName);
  }

  public async Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
  {
    session.Store(shoppingCart);
    await session.SaveChangesAsync();
    return shoppingCart;
  }
  public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
  {
    session.Delete<ShoppingCart>(userName);
    await session.SaveChangesAsync();
    return true;
  }
}
