using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AmadeusSDK.Tests;

[TestClass]
public sealed class IntegrationTests
{

    [TestMethod]
    public async Task SearchFlights()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
