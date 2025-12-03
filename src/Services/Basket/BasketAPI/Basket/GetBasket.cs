namespace BasketAPI.Basket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
public record GetBasketResult(ShoppingCart Cart);
internal class GetBasketQueryHandler(IBasketRepository repo) : IQueryHandler<GetBasketQuery, GetBasketResult>
{
  public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
  {
    var basket = await repo.GetBasket(query.UserName, cancellationToken);

    return new GetBasketResult(basket);
  }
}


//public record GetBasketRequest(string UserName);
public record GetBasketResponse(ShoppingCart Cart);
public class GetBasketEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
    {
      var result = await sender.Send(new GetBasketQuery(userName));

      var response = result.Adapt<GetBasketResponse>();

      return Results.Ok(response);
    })
        .WithName("GetCartByUserName")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Cart By UserName")
        .WithDescription("Get Cart By UserName");
  }
}