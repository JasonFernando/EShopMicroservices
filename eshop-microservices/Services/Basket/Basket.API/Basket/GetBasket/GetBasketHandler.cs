// ARQUITECTURA: Vertical Slices.
// En lugar de tener carpetas "Services" o "Managers", agrupamos por funcionalidad (Feature Folder).
namespace Basket.API.Basket.GetBasket
{
    // PATRÓN: CQRS (Query Side) & DTO Inmutable.
    // 1. 'record': Define un objeto inmutable optimizado para transporte de datos.
    // 2. 'IQuery<>': Marca esto como una operación de LECTURA (sin efectos secundarios), 
    //    típico de la librería MediatR o BuildingBlocks personalizados.
    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

    // PATRÓN: Wrapper / Envelope.
    // Envuelve la respuesta para mantener consistencia en los retornos del API.
    public record GetBasketResult(ShoppingCart Cart);

    // PATRÓN: Mediator (Handler) & Dependency Injection.
    // 1. Implementa la lógica para procesar un request específico (GetBasketQuery).
    // 2. PRIMARY CONSTRUCTOR (C# 12): Inyección de dependencias concisa. 
    //    'repository' se inyecta automáticamente sin declarar campos privados manuales.
    public class GetBasketQueryHandler(IBasketRepository repository)
        : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        // PATRÓN: Task-based Asynchronous Pattern (TAP).
        // Método asíncrono que permite liberar el hilo mientras espera I/O (Base de datos/Caché).
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            // PATRÓN: Repository & Decorator (Proxy).
            // 1. Abstracción: El handler no sabe si va a Postgres (Marten) o Redis.
            // 2. Decorator (Oculto): Gracias a Scrutor, esta variable 'repository' es en realidad
            //    una instancia de 'CachedBasketRepository', que intercepta la llamada 
            //    para revisar Redis antes de ir a la BD.
            // 3. CancellationToken: Permite cancelar la operación si el usuario cierra el navegador.
            var basket = await repository.GetBasket(query.UserName, cancellationToken);

            // Mapeo simple de Dominio a DTO de respuesta.
            return new GetBasketResult(basket);
        }
    }
}