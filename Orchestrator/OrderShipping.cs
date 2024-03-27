namespace Orchestrator;

using IntegrationEvents;

using MassTransit;

public class OrderShipping : MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderShipping> _logger;

    public State Placed { get; private set; } = null!;
    public State Paid { get; private set; } = null!;
    public State Prepared { get; private set; } = null!;

    public Event<OrderPlaced> OrderPlaced { get; private set; } = null!;
    public Event<OrderPaid> OrderPaid { get; private set; } = null!;
    public Event<OrderPrepared> OrderPrepared { get; private set; } = null!;

    public OrderShipping(ILogger<OrderShipping> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);

        Event(() => OrderPlaced, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => OrderPaid, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => OrderPrepared, e => e.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
            When(OrderPlaced)
                .Then(context =>
                {
                    context.Saga.Customer = new(context.Message.CustomerId,
                        context.Message.FirstName,
                        context.Message.LastName,
                        context.Message.Email,
                        context.Message.PhoneNumber);

                    _logger.LogInformation("Order placed: {id}", context.Message.OrderId);
                })
                .TransitionTo(Placed));

        During(Placed,
            When(OrderPaid)
                .Then(context => _logger.LogInformation("Order paid: {id}", context.Message.OrderId))
                .TransitionTo(Paid),
            When(OrderPrepared)
                .Then(context => _logger.LogInformation("Order prepared: {id}", context.Message.OrderId))
                .TransitionTo(Prepared));

        During(Paid,
            When(OrderPrepared)
                .ThenAsync(ShipOrder)
                .Finalize());

        During(Prepared,
            When(OrderPaid)
                .ThenAsync(ShipOrder)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    private async Task ShipOrder(BehaviorContext<OrderState, OrderPrepared> context)
    {
        await context.Publish(
            new OrderShipped(context.Message.OrderId,
                context.Saga.Customer.FirstName,
                context.Saga.Customer.LastName,
                context.Saga.Customer.PhoneNumber));
    }

    private async Task ShipOrder(BehaviorContext<OrderState, OrderPaid> context)
    {
        await context.Publish(
            new OrderShipped(context.Message.OrderId,
                context.Saga.Customer.FirstName,
                context.Saga.Customer.LastName,
                context.Saga.Customer.PhoneNumber));
    }
}
