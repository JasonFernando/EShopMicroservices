using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductHandler(IDocumentSession session) 
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Business logic to create a product
            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageUrl = command.ImageUrl,
                Price = command.Price
            };

            //TO DO
            //save database
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            //return result
            return new CreateProductResult(product.Id);
        }
    }

    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageUrl, decimal Price)
        : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);
}
