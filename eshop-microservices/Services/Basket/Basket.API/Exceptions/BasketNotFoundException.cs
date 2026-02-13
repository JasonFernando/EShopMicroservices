namespace Basket.API.Exceptions
{
    // PATRÓN: Custom Domain Exception (Excepción de Dominio Personalizada).
    // Heredamos de 'NotFoundException' (que probablemente vive en tus BuildingBlocks).
    // ESTO PERMITE: Que tu Middleware de manejo de errores global capture esta excepción
    // y sepa automáticamente que debe devolver un HTTP 404 (Not Found) al cliente,
    // sin tener que escribir try-catch en tus controladores.
    public class BasketNotFoundException : NotFoundException
    {
        // CONSTRUCTOR: Abstracción del Mensaje.
        // Solo pedimos el dato relevante ('userName') y le pasamos los detalles a la clase base.
        // base("Basket", userName): Le dice al padre: "La entidad 'Basket' con la llave 'userName' no apareció".
        public BasketNotFoundException(string userName) : base("Basket", userName)
        {
            // El cuerpo está vacío porque la lógica de formatear el mensaje de error 
            // ("Entity 'Basket' ({userName}) was not found.") ya está resuelta en la clase padre.
            // Esto mantiene tu código de negocio limpio y estandarizado.
        }
    }
}