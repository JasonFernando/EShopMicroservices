using BuildingBlocks.Behavious;
using BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

//builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// add global exception
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// configure the HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.Run();
