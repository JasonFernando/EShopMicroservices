using BuildingBlocks.Behavious;
using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// =================================================================================
// 1. CAPA DE PRESENTACIÓN (ENDPOINTS)
// =================================================================================
// LIBRERÍA: Carter.
// PATRÓN: Vertical Slice Architecture.
// En lugar de usar Controllers pesados, Carter escanea tu proyecto buscando clases que
// implementen ICarterModule para registrar rutas (Minimal APIs) automáticamente.
builder.Services.AddCarter();

// =================================================================================
// 2. CAPA DE APLICACIÓN (MEDIATR & PIPELINES)
// =================================================================================
// LIBRERÍA: MediatR.
// PATRÓN: Mediator & Pipeline (Chain of Responsibility).
builder.Services.AddMediatR(config =>
{
    // Escanea el ensamblado actual para encontrar automáticamente todos los
    // IRequest, IRequestHandler, IQuery, ICommand, etc.
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);

    // PATRÓN: AOP (Aspect Oriented Programming) / Cross-Cutting Concerns.
    // Estos "Behaviors" son middlewares que envuelven a tus Handlers.
    // Funcionan como matrioskas (muñecas rusas):
    // Request -> [Logging] -> [Validation] -> [HANDLER] -> [Validation] -> [Logging] -> Response

    // LoggingBehavior: Registra automáticamente la entrada y salida de cada comando/query.
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));

    // ValidationBehavior: Intercepta el request, ejecuta validaciones (FluentValidation)
    // y si falla, lanza una excepción antes de que el Handler se entere.
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// =================================================================================
// 3. CAPA DE DATOS (PERSISTENCIA & MARTEN)
// =================================================================================
// LIBRERÍA: Marten.
// PATRÓN: Document Database (sobre PostgreSQL).
// Transforma Postgres en una base de datos NoSQL. Guardamos objetos .NET completos como JSON.
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);

    // Configuración de Esquema: Le decimos a Marten cuál propiedad del objeto
    // debe usar como Primary Key (Identity) en la base de datos.
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions(); // Optimización para operaciones rápidas sin tracking excesivo.

// =================================================================================
// 4. ESTRATEGIA DE CACHÉ (DECORATOR & REDIS)
// =================================================================================
// Paso A: Registramos el repositorio base (Marten).
// DI LIFETIME: Scoped (Se crea una instancia por cada petición HTTP).
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// LIBRERÍA: Scrutor.
// PATRÓN: Decorator / Proxy.
// MAGIA: Esto intercepta la línea de arriba. Cuando alguien pida IBasketRepository,
// el contenedor DI entregará una instancia de 'CachedBasketRepository'.
// Dentro, 'CachedBasketRepository' tendrá inyectado el 'BasketRepository' original.
// Esto permite agregar caché SIN TOCAR el código del repositorio base.
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

// Configuración del servidor Redis.
// Implementa la interfaz IDistributedCache nativa de .NET.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // Recomendación: Agregar 'InstanceName' aquí para evitar colisiones de llaves si tienes varios servicios.
});

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
    .ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

// =================================================================================
// 5. MANEJO DE ERRORES GLOBAL
// =================================================================================
// PATRÓN: Global Exception Handling (Middleware).
// Registra tu 'CustomExceptionHandler' para capturar cualquier excepción no controlada
// y transformarla en una respuesta HTTP estandarizada (ProblemDetails RFC 7807).
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// =================================================================================
// 6. OBSERVABILIDAD (HEALTH CHECKS)
// =================================================================================
// PATRÓN: Health Check (Monitorización).
// El microservicio expone un endpoint para decir "Estoy vivo".
// Además, verifica activamente si sus dependencias críticas (Postgres y Redis) responden.
// Si Redis se cae, el servicio se reportará como "Unhealthy".
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// =================================================================================
// 7. PIPELINE DE PETICIONES HTTP (MIDDLEWARES)
// =================================================================================

// Mapea los endpoints definidos en las clases de Carter.
app.MapCarter();

// Activa el middleware de manejo de excepciones que configuramos arriba.
// El lambda vacío opciones => { } es necesario por la firma del método, aunque usemos la config por defecto.
app.UseExceptionHandler(options => { });

// Expone la ruta "/health" para que orquestadores (Kubernetes/Docker) o monitores sepan el estado.
// 'ResponseWriter': Formatea la respuesta JSON para que sea compatible con dashboards gráficos (HealthChecks-UI).
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();