namespace CatalogAPI.Products.GetProductByCategory;

public record GetProductsByCategoryResult(IEnumerable<Product> Products);
public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
internal class GetProductsByCategoryHandler(IDocumentSession session, ILogger<GetProductsByCategoryHandler> logger) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
  public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
  {
    logger.LogInformation("GetProductsByCategoryHandler.Handle called with {@Query}", query);

    var result = await session.Query<Product>().Where(p => p.Category.Contains(query.Category)).ToListAsync();

    return new GetProductsByCategoryResult(result);
  }
}


//public record GetProductsByCategoryRequest();
public record GetProductsByCategoryResponse(IEnumerable<Product> Products);

public class GetProductsByCategoryEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/product/category/{category}", async (string category, ISender sender) =>
    {
      var result = await sender.Send(new GetProductsByCategoryQuery(category));

      var response = result.Adapt<GetProductsByCategoryResponse>();

      return Results.Ok(response);
    })
       .WithName("GetProductsByCategory")
       .Produces<GetProductsByCategoryResponse>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("Get Product by its Category")
       .WithDescription("Get Product by its Category");
  }
}
