
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.HPRtree;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    public class GetProductByCatergoryHandler(IDocumentSession session, ILogger<GetProductByCategoryQuery> logger) : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation($"GetProductByCatergoryHandler.Hanlde called with {query}");

            var products = await session.Query<Product>().Where(p => p.Category.Contains(query.Category)).ToListAsync(cancellationToken);

            return new GetProductByCategoryResult(products);
        }
    }
}
