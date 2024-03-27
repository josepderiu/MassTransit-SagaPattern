# Saga Pattern Learning Project

This project is a learning project focused on implementing the Saga Pattern using MassTransit in a .NET environment.

Please note that this project uses in-memory persistence for the Saga for the sake of simplicity. It is not recommended for use in a production environment.

## Introduction

The Saga Pattern is a design pattern used in distributed systems to manage long-running transactions that span multiple services. It helps to maintain data consistency and handle failures gracefully.

## Getting Started

To get started with this project, follow these steps:

1. Clone the repository: `git clone https://github.com/your-username/your-repo.git`
2. Install the required dependencies: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the tests: `dotnet test`
5. Run the project: `dotnet run --project .\Orchestrator\Orchestrator.csproj`

## Validating the Saga

To validate the Saga, you can follow these steps:

1. Start by sending a POST request to the `/orders` endpoint. This will initiate the Saga. The API will return an `OrderId` which you will use in the subsequent steps.

```http
POST http://localhost:5236/orders
Content-Type: application/json
```

2. Once you have the OrderId, you can then send a POST request to the `/payments` endpoint with the `OrderId` as a query parameter. This will simulate the payment process.

```http
POST http://localhost:5236/payments?orderId={OrderId}
Content-Type: application/json
```

3. After the payment process, you can then send a POST request to the `/warehouse/orders` endpoint with the `OrderId` as a query parameter. This will simulate the warehouse process and complete the Saga.

```http
POST http://localhost:5236/warehouse/orders?orderId={OrderId}
Content-Type: application/json
```

Replace `{OrderId}` with the actual `OrderId` you received from the /orders endpoint.

## Resources

For more information about the Saga Pattern and MassTransit, refer to the following resources:

- [Saga Pattern - Microsoft Docs](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/saga/saga)
- [MassTransit Documentation](https://masstransit-project.com/)

## Contributing

Contributions to this learning project are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
