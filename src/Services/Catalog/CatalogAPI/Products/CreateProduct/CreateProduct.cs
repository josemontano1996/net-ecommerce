namespace CatalogAPI.Products.CreateProduct;


public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
public record CreateProductResult(Guid ProductId);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
  public CreateProductCommandValidator()
  {
    RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required").Length(2, 150).WithMessage("Name must be between 2 and 15 characters");
    RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
    RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
    RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
  }
}
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
