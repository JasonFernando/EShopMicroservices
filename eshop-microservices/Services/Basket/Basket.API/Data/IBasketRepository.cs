namespace Basket.API.Data
{
    // PATRÓN: Repository Pattern (Abstracción / Contrato).
    // 1. Principio de Inversión de Dependencias (DIP):
    //    Los módulos de alto nivel (tus Handlers CQRS) no dependen de los detalles de bajo nivel
    //    (Marten o Redis), sino de esta abstracción.
    // 2. Extensibilidad (Open/Closed Principle):
    //    Esta interfaz es la que permite que 'Scrutor' inyecte el 'CachedBasketRepository' 
    //    en lugar del repositorio real sin romper el código.
    // 3. Testability: Facilita la creación de Mocks (Moq/NSubstitute) para pruebas unitarias.
    public interface IBasketRepository
    {
        // TÉCNICA: Asynchronous Programming (TAP).
        // Uso de Task<> para operaciones I/O Bound (Base de datos/Red) no bloqueantes.

        // BEST PRACTICE: CancellationToken.
        // Se propaga el token desde el Endpoint -> Handler -> Repositorio.
        // Si el usuario cancela la petición HTTP, la consulta a la BD se cancela automáticamente,
        // liberando conexiones y recursos del servidor. '= default' lo hace opcional.
        Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default);

        // DOMINIO: Persistencia de Estado.
        // Recibe la entidad de dominio completa (ShoppingCart) para ser serializada.
        // Funciona como un "Upsert" (Update o Insert) dependiendo de si ya existe.
        Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default);

        // OPERACIÓN: Eliminación.
        // Retorna un booleano para confirmar si la operación tuvo efecto (si el carrito existía y se borró).
        Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default);
    }
}