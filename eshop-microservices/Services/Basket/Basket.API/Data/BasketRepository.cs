namespace Basket.API.Data
{
    // PATRÓN: Repository Pattern (Implementación).
    // Su propósito es aislar el dominio de la base de datos. El resto de la app
    // solo conoce la interfaz 'IBasketRepository', sin saber que usamos Marten o Postgres por debajo.

    // LIBRERÍA: Marten.
    // Estamos usando Marten para tratar a PostgreSQL como una Base de Datos de Documentos (NoSQL).
    // Esto guarda la clase 'ShoppingCart' como un JSON binario (JSONB) en lugar de filas y columnas tradicionales.

    // SINTAXIS: Primary Constructor (C# 12).
    // Inyectamos 'IDocumentSession' directamente en la declaración de la clase.
    // 'IDocumentSession' es la unidad de trabajo de Marten (equivalente al DbContext de EF Core).
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            // MARTEN: LoadAsync (Lectura por ID).
            // Busca eficientemente en la columna indexada (Primary Key) el documento JSON
            // que coincide con 'userName' y lo deserializa automáticamente a un objeto 'ShoppingCart'.
            var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);

            // TÉCNICA: Guard Clause / Fail Fast.
            // En lugar de devolver un nulo y causar errores aguas abajo,
            // validamos inmediatamente. Si no existe, lanzamos una excepción de dominio controlada.
            return basket is null ? throw new BasketNotFoundException(userName) : basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            // MARTEN: Upsert (Insert or Update).
            // El método .Store() es inteligente:
            // - Si el ID del carrito no existe en la BD -> Hace un INSERT.
            // - Si el ID ya existe -> Hace un UPDATE (sobreescribe el JSON).
            // Esto simplifica la lógica al no tener que preguntar "if exists" antes.
            session.Store(basket);

            // PATRÓN: Unit of Work (Transaccionalidad).
            // Marten acumula todos los cambios en memoria (session).
            // Solo cuando llamamos a 'SaveChangesAsync' se abre la conexión a Postgres
            // y se comitea la transacción.
            await session.SaveChangesAsync(cancellationToken);
            return basket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            // MARTEN: Delete por ID.
            // Marca el objeto para ser eliminado. Marten sabe qué tabla tocar basándose en el tipo genérico <ShoppingCart>.
            session.Delete<ShoppingCart>(userName);

            // Persistencia