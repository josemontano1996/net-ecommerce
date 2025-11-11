namespace CatalogAPI.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
public record GetProductByIdResult(Product Product);

internal class GetProductByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
  public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
  {
    var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

    if (product is null)
    {
      throw new ProductNotFoundException(query.Id);
    }

    return new GetProductByIdResult(product);
  }
}

// public record GetProductByIdRequest();
public record GetProductByIdResponse(Product Product);

public class GetProductByIdEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/product/{id}", async (Guid Id, ISender sender) =>
    {
      var result = await sender.Send(new GetProductByIdQuery(Id));

      var response = result.Adapt<GetProductByIdResponse>();

      return Results.Ok(response);
    })
      .WithName("GetProductById")
      .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .ProducesProblem(StatusCodes.Status404NotFound)
      .WithSummary("Get Product by its Id")
      .WithDescription("Get Product by Id");
  }
}
