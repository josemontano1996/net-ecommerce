namespace CatalogAPI.Products.DeleteProductById;

public record DeleteProductByIdResult(bool IsSucess);
public record DeleteProductByIdCommand(Guid Id) : ICommand<DeleteProductByIdResult>;

public class DeleteProductByIdCommandValidator : AbstractValidator<DeleteProductByIdCommand>
{
  public DeleteProductByIdCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required");
  }
}

internal class DeleteProductByIdCommandHandler(IDocumentSession session, ILogger<DeleteProductByIdCommandHandler> logger) : ICommandHandler<DeleteProductByIdCommand, DeleteProductByIdResult>
{
  public async Task<DeleteProductByIdResult> Handle(DeleteProductByIdCommand command, CancellationToken cancellationToken)
  {
    logger.LogInformation("DeleteProductByIdCommandHandler.Handle called with {@Command}", command);

    session.Delete<Product>(command.Id);

    await session.SaveChangesAsync(cancellationToken);

    return new DeleteProductByIdResult(true);
  }
}

//public record DeleteProductByIdRequest(Guid Id);
public record DeleteProductByIdResponse(bool IsSucess);
public class DeleteProductByIdEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapDelete("/product/{id}", async (Guid Id, ISender sender) =>
    {
      var command = new DeleteProductByIdCommand(Id);

      var result = await sender.Send(command);

      var response = result.Adapt<DeleteProductByIdResponse>();

      return Results.Ok(response);
    })
      .WithName("DeleteProductById")
      .Produces<DeleteProductByIdResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("Delete Product")
      .WithDescription("Delete Product");
  }
}