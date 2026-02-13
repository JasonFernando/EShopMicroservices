using MediatR;

namespace BuildingBlocks.CQRS
{
    // ARQUITECTURA: Abstracción de CQRS (Query Side).
    // SELECCIÓN DE DISEÑO: Interfaz Genérica Covariante.

    // 1. 'out TResponse': La palabra clave 'out' habilita la COVARIANZA.
    //    Esto permite que un IQuery<String> pueda ser tratado como un IQuery<Object>.
    //    Es crucial para que los Pipelines y Behaviors genéricos puedan procesar
    //    cualquier tipo de query sin saber el tipo exacto de retorno en tiempo de compilación.

    // 2. ': IRequest<TResponse>': Herencia de MediatR.
    //    Conectamos nuestra abstracción con la infraestructura de MediatR.
    //    Esto permite que el 'ISender' o 'IMediator' sepa cómo enrutar este objeto
    //    hacia su respectivo Handler.

    // 3. 'where TResponse : notnull': Constraint (Restricción).
    //    Forzamos a que el resultado NUNCA sea nulo. En CQRS, una Query siempre
    //    debería devolver un resultado o una colección vacía, o lanzar una excepción,
    //    pero no 'null' silenciosos, mejorando la seguridad del código (Null Safety).
    public interface IQuery<out TResponse> : IRequest<TResponse>
        where TResponse : notnull
    {
        // Esta interfaz está vacía intencionalmente (Marker Interface).
        // Su único propósito es definir el "Rol" del objeto: "Soy una petición de lectura".
    }
}
