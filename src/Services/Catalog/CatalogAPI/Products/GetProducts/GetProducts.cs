
using Marten.Pagination;

namespace CatalogAPI.Products.GetProducts;

public record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10) : IQuery<GetProductsResult>;
public record GetProductsResult(IEnumerable<Product> Products);
internal class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductsResult>
{
  public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
  {
    var products = await session.Query<Product>()
      .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

    return new GetProductsResult(products);
  }
}

public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);
public record GetProductResponse(IEnumerable<Product> Products);
public class GetProductsEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/product", async ([AsParameters] GetProductsRequest request, ISender sender) =>
    {
      var query = request.Adapt<GetProductsQuery>();

      var result = await sender.Send(query);

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

