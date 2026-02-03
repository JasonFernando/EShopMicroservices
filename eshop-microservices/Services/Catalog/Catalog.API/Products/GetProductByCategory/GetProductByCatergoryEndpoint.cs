
using Catalog.API.Products.CreateProduct;
using Catalog.API.Products.GetProductById;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductsByCategoryResponse(IEnumerable<Product> Products);
    public class GetProductByCatergoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));

                var response = result.Adapt<GetProductsByCategoryResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductsByCategory")
            .Produces<GetProductsByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Category")
            .WithDescription("Get Product By Category");
        }
    }
}
