namespace Orchestrator;

using MassTransit;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = null!;

    public Customer Customer { get; set; } = null!;
}

public sealed class Customer(Guid CustomerId,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber)
{
    public Guid CustomerId { get; set; } = CustomerId;
    public string FirstName { get; set; } = FirstName;
    public string LastName { get; set; } = LastName;
    public string Email { get; set; } = Email;
    public string PhoneNumber { get; set; } = PhoneNumber;
}