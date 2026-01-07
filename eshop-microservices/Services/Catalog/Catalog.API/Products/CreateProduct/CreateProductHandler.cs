using BuildingBlocks.CQRS;
using Catalog.API.Models;
using MediatR;

namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductHandler 
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

            //return result
            return new CreateProductResult(Guid.NewGuid());
        }
    }

    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageUrl, decimal Price)
        : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);
}
