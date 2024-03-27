using Bogus;

using IntegrationEvents;

using MassTransit;

using Orchestrator;
using Orchestrator.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderShippedConsumer>();

    x.AddSagaStateMachine<OrderShipping, OrderState>()
        .Endpoint(e =>
        {
            e.Name = "shipping";
            e.ConcurrentMessageLimit = 8;
        });

    x.UsingInMemory((ctx, cfg) =>
    {
        cfg.UseJsonSerializer();
        cfg.UseJsonDeserializer();

        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.RegisterInMemorySagaRepository<OrderState>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOrderEndpoints();

app.Run();

internal static class WebApplicationExtensions
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        var orders = app.MapGroup("/orders")
            .AllowAnonymous();

        var payments = app.MapGroup("/payments")
            .AllowAnonymous();

        var preparations = app.MapGroup("/warehouse/orders")
            .AllowAnonymous();

        orders.MapPost("", async (IPublishEndpoint publisher,
            CancellationToken cancellationToken) =>
        {
            var orderId = Guid.NewGuid();

            var person = new Faker().Person;

            await publisher.Publish(
                new OrderPlaced(orderId,
                    Guid.NewGuid(),
                    person.FirstName,
                    person.LastName,
                    person.Email,
                    person.Phone),
                cancellationToken);

            return Results.Ok(orderId);
        })
        .WithOpenApi();

        payments.MapPost("", async (Guid orderId,
            IPublishEndpoint publisher,
            CancellationToken cancellationToken) =>
        {
            await publisher.Publish(
                new OrderPaid(orderId),
                cancellationToken);

            return Results.Accepted();
        })
        .WithOpenApi();

        preparations.MapPost("", async (Guid orderId,
            IPublishEndpoint publisher,
            CancellationToken cancellationToken) =>
        {
            await publisher.Publish(
                new OrderPrepared(orderId),
                cancellationToken);

            return Results.Accepted();
        })
        .WithOpenApi();

        return app;
    }
}

public partial class Program { }