using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery() : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);
    public class GetProductsQueryHandler(IDocumentSession session, ILogger<GetProductsQueryHandler> logger) : IqueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductHanlder called with {@Query}", query);

            var products = await session.Query<Product>().ToListAsync(cancellationToken);

            return new GetProductsResult(products);
        }
    }
}
