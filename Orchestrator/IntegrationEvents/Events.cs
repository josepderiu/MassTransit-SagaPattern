namespace IntegrationEvents;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, string FirstName, string LastName, string Email, string PhoneNumber);

public sealed record OrderPaid(Guid OrderId);

public sealed record OrderPrepared(Guid OrderId);

public sealed record OrderShipped(Guid OrderId, string FirstName, string LastName, string PhoneNumber);