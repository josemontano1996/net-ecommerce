using BuildingBlocks.CQRS;

namespace CatalogAPI.Products.CreateProduct
{

    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);
    internal class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Business logic to create a product
            var product = new ProducesResponseTypeMetadata ct
            // Save to database

            // Return result
            throw new NotImplementedException();
        }
    }
}
