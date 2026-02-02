
using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    public class GetProductByCatergoryHandler(IDocumentSession session, ILogger<GetProductByCategoryQuery> logger) : IqueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation($"GetProductByCatergoryHandler.Hanlde called with {query}");

            var products = await session.Query<Product>().ToListAsync(cancellationToken);
        }
    }
}
