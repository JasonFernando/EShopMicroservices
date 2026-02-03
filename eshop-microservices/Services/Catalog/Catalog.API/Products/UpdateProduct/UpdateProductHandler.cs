
using Catalog.API.Products.GetProductByCategory;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageUrl, decimal Price) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);
    public class UpdateProductHandler(IDocumentSession session, ILogger<GetProductByCategoryQuery> logger) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
