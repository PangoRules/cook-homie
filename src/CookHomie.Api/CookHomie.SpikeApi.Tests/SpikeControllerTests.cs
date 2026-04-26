using System.Net;
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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello from C#", message);
    }
}
