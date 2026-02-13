using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    // PATRÓN: Decorator (o Caching Proxy).
    // 1. Implementa la misma interfaz 'IBasketRepository' que la clase a la que envuelve.
    // 2. Recibe por inyección (Primary Constructor) la implementación real ('repository').
    // 3. Intercepta las llamadas para añadir la lógica de Caché (Redis) antes o después
    //    de llamar a la base de datos.
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            // PATRÓN: Cache-Aside (Lazy Loading).
            // Paso 1: Preguntamos primero al Caché (Operación rápida en memoria/Redis).
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);

            // HIT: Si el dato existe en Redis, lo deserializamos y retornamos inmediatamente.
            // Esto evita el viaje costoso a la base de datos (PostgreSQL).
            if (!string.IsNullOrEmpty(cachedBasket))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

            // MISS: Si no está en caché, llamamos al Repositorio Real (Base de Datos).
            // Aquí es donde el Decorador delega el trabajo pesado al objeto envuelto.
            var basket = await repository.GetBasket(userName, cancellationToken);

            // REFRESH: Guardamos el resultado en Redis para futuras peticiones.
            // Serializamos el objeto a JSON porque Redis almacena Strings/Bytes.
            // *NOTA DE ARQUITECTO*: Aquí sería ideal agregar 'DistributedCacheEntryOptions' 
            // con un tiempo de expiración (TTL) para que el dato no viva para siempre.
            await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            // PATRÓN: Write-Through (Estrategia de consistencia).
            // Paso 1: Guardamos en la "Fuente de la Verdad" (PostgreSQL) primero.
            // Si esto falla, la operación se cancela y no ensuciamos el caché.
            await repository.StoreBasket(basket, cancellationToken);

            // Paso 2: Actualizamos el caché inmediatamente.
            // Esto asegura que la próxima lectura (GetBasket) obtenga la versión más nueva
            // sin tener que ir a la base de datos.
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);

            return basket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            // CONSISTENCIA: Eliminación coordinada.
            // Borramos de la base de datos.
            await repository.DeleteBasket(userName, cancellationToken);

            // Borramos del caché.
            // Es crucial para evitar "Datos Fantasmas" (que el usuario vea un carrito que ya borró).
            await cache.RemoveAsync(userName, cancellationToken);

            return true;
        }
    }
}