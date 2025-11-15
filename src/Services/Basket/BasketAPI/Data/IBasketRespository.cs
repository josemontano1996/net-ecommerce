namespace BasketAPI.Data;

public interface IBasketRespository
{
  Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellation = default);
  Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
  Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default);
}
