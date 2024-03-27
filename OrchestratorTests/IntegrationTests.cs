namespace OrchestratorTests;

using Microsoft.AspNetCore.Mvc.Testing;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PostOrders_ReturnsOk()
    {
        var response = await _client
            .PostAsync("/orders", null);

        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostPayments_ReturnsAccepted()
    {
        var response = await _client
            .PostAsync($"/payments?orderId={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task PostShippments_ReturnsAccepted()
    {
        var response = await _client
            .PostAsync($"/warehouse/orders?orderId={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
    }
}