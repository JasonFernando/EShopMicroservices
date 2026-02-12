using Catalog.API.Products.GetProducts;
using Marten;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>; 
    public record GetProductByIdResult(Product Product);
    public class GetProductByIdQueryHandler(IDocumentSession session, ILogger<GetProductByIdQueryHandler> logger) 
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation($"GetProductByIdQueryHandler.Hanlde called with {query}");

            var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

            if(product is null)
            {
                throw new ProductNotFoundException(query.Id);
            }

            return new GetProductByIdResult(product);
        }
    }
}
