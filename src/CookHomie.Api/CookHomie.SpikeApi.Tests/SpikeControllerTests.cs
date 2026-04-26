using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CookHomie.SpikeApi.Tests;

public class SpikeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SpikeControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHello_ReturnsOk()
    {
        var response = await _client.GetAsync("/spike/hello");
        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        var message = json.RootElement.GetProperty("message").GetString();
        var timestampValue = json.RootElement.GetProperty("timestamp").GetString();
        var timestampIsParseable = DateTimeOffset.TryParse(timestampValue, out _);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello from C#", message);
        Assert.False(string.IsNullOrWhiteSpace(timestampValue));
        Assert.True(timestampIsParseable);
    }

    [Fact]
    public async Task GetInventory_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/inventory");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostInventory_CreatesItem_ThenGetReturnsIt()
    {
        var request = new
        {
            name = "Milk",
            category = "dairy",
            location = "Fridge",
            quantity = 1,
            unit = "liter",
            isOpened = false
        };

        var postResponse = await _client.PostAsJsonAsync("/api/inventory", request);
        var created = await postResponse.Content.ReadFromJsonAsync<InventoryItemResponse>();

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("Milk", created!.Name);

        var getResponse = await _client.GetAsync("/api/inventory");
        var items = await getResponse.Content.ReadFromJsonAsync<List<InventoryItemResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(items);
        Assert.Contains(items!, i => i.Id == created.Id && i.Name == "Milk");
    }

    private sealed class InventoryItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
