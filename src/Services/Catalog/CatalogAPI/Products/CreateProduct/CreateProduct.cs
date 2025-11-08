
namespace CatalogAPI.Products.CreateProduct;


public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
public record CreateProductResult(Guid ProductId);
internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
  public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
  {
    // Create product entity
    var product = new Product
    {
      Name = command.Name,
      Category = command.Category,
      Description = command.Description,
      ImageFile = command.ImageFile,
      Price = command.Price
    };

    // Save to database
    session.Store(product);
    await session.SaveChangesAsync(cancellationToken);

    // Return result
    return new CreateProductResult(product.Id);
  }
}

public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
public record CreateProductResponse(Guid ProductId);
public class CreateProductEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapPost("/product", async (CreateProductRequest request, ISender sender) =>
    {
      var command = request.Adapt<CreateProductCommand>();

      var result = await sender.Send(command);

      var response = result.Adapt<CreateProductResponse>();

      return Results.Created($"/product/{response.ProductId}", response);
    })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .WithDescription("Create Product");
  }
}
