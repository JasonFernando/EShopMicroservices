using Catalog.API.Products.GetProducts;
using Marten;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResult>; 
    public record GetProductByIdResult(Product Product);
    internal class GetProductByIdQueryHandler(IDocumentSession session, ILogger<GetProductByIdQueryHandler> logger) 
        : IqueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation($"GetProductByIdQueryHandler.Hanlde called with {query}");

            var product = await session.LoadAsync<Product>(query.id, cancellationToken);

            if(product is null)
            {
                throw new ProductNotFloundException();
            }

            return new GetProductByIdResult(product);
        }
    }
}
