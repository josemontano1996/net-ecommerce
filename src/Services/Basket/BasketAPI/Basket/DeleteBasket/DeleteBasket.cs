using Mapster;

namespace BasketAPI.Basket.DeleteBasket;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);
public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
  public DeleteBasketCommandValidator()
  {
    RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
  }
}

internal class DeleteBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
  public async Task<DeleteBasketResult> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
  {
    // TODO: delete basket from database and cache

    return new DeleteBasketResult(true);
  }
}


// public record DeleteBasketRequest(string UserName);
public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
    {
      var result = await sender.Send(new DeleteBasketCommand(userName));

      var response = result.Adapt<DeleteBasketResponse>();

      return Results.Ok(response);
    })
        .WithName("DeleteBasket")
        .Produces<DeleteBasketResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("DeleteBasket")
        .WithDescription("DeleteBasket");
  }
}
