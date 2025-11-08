
namespace CatalogAPI.Products.GetProducts;

public record GetProductsQuery : IQuery<GetProductsResult>;
public record GetProductsResult(IEnumerable<Product> Products);
internal class GetProductsQueryHandler(IDocumentSession session, ILogger<GetProductsQueryHandler> logger) : IQueryHandler<GetProductsQuery, GetProductsResult>
{
  public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
  {
    logger.LogInformation("GetProductsQueryHandler.Handle called with {@Query}", query);

    var products = await session.Query<Product>().ToListAsync(cancellationToken);

    return new GetProductsResult(products);
  }
}

public record GetProductResponse(IEnumerable<Product> Products);
public class GetProductsEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/product", async (ISender sender) =>
    {
      var result = await sender.Send(new GetProductsQuery());

      var response = result.Adapt<GetProductResponse>();

      return Results.Ok(response);
    })
      .WithName("GetProducts")
      .Produces<GetProductResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("Get Products")
      .WithDescription("Get Products");
  }
}

