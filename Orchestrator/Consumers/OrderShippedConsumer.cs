namespace Orchestrator.Consumers;

using System.Threading.Tasks;

using IntegrationEvents;

using MassTransit;

public class OrderShippedConsumer(ILogger<OrderShippedConsumer> logger) : IConsumer<OrderShipped>
{
    private readonly ILogger<OrderShippedConsumer> _logger = logger;

    public Task Consume(ConsumeContext<OrderShipped> context)
    {
        _logger.LogInformation("Order: {id} shipped.\n\tCustomer: {name}\n\tPhone: {phone}", 
            context.Message.OrderId,
            $"{context.Message.LastName}, {context.Message.FirstName}",
            context.Message.PhoneNumber);

        return Task.CompletedTask;
    }
}