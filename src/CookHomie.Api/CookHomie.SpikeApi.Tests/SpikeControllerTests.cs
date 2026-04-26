using System.Net;
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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
