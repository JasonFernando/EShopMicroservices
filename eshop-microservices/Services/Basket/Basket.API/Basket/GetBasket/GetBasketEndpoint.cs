// ARQUITECTURA: Vertical Slice Architecture.
// Todo lo relacionado con la feature "GetBasket" (Modelos, Endpoints, Lógica) vive en este namespace.
namespace Basket.API.Basket.GetBasket
{
    // DTO (Data Transfer Object): Respuesta pública del API.
    // 'record': Inmutabilidad y sintaxis concisa para definir la estructura de datos.
    public record GetBasketResponse(ShoppingCart Cart);

    // LIBRERÍA: Carter.
    // Implementar 'ICarterModule' permite organizar los Endpoints de forma modular y limpia.
    // Carter escanea la assembly al inicio y registra estas rutas automáticamente, evitando ensuciar el Program.cs.
    public class GetBasketEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // TÉCNICA: Minimal APIs.
            // Define rutas HTTP de alto rendimiento con menos "ceremonia" que los Controllers tradicionales.
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                // PATRÓN: Mediator (MediatR).
                // Desacoplamiento total: El Endpoint no conoce la lógica de negocio ni el repositorio.
                // Solo envía un mensaje (Query) y espera el resultado.
                // 'ISender': Interfaz de MediatR para enviar comandos/queries.
                var result = await sender.Send(new GetBasketQuery(userName));

                // LIBRERÍA: Mapster.
                // Mapeo Objeto-a-Objeto de alto rendimiento.
                // Convierte el resultado interno (dominio) a la respuesta pública (DTO)
                // sin necesidad de escribir código de asignación manual.
                var response = result.Adapt<GetBasketResponse>();

                return Results.Ok(response);
            })
            // METADATA / OPENAPI (Swagger):
            // Estas líneas enriquecen la documentación automática de tu API.
            .WithName("GetBasket") // Nombre único para generación de enlaces (Link Generation).
            .Produces<GetBasketResponse>(StatusCodes.Status200OK) // Documenta el tipo de retorno exitoso.
            .ProducesProblem(StatusCodes.Status400BadRequest) // Documenta posibles errores.
            .WithSummary("Get basket") // Resumen corto para la UI de Swagger.
            .WithDescription("Get basket"); // Descripción detallada.
        }
    }
}